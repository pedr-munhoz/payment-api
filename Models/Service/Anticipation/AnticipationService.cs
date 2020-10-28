using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using payment_api.Infrastructure.Database;
using payment_api.Models.Result;

namespace payment_api.Models.Service
{
    public class AnticipationService : IAnticipationService
    {
        private const double TaxRate = 0.962;
        private readonly ServerDbContext _dbContext;

        private readonly IResultService _resultService;

        public AnticipationService(ServerDbContext dbContext, IResultService resultService)
        {
            _dbContext = dbContext;
            _resultService = resultService;
        }

        public async Task<AnticipationResult> Create(List<int> paymentIds, DateTime solicitationDate)
        {
            // Check if there's a open solicitation
            var openAnalysis = await _dbContext.Set<AnticipationAnalysis>()
                                        .Where(x => x.EndDate == null)
                                        .FirstOrDefaultAsync();
            if (openAnalysis != null)
                return _resultService.GenerateFailedResult("Cannot open solicitation without finilizing the current one.", true);

            var payments = await _dbContext.Set<PaymentEntity>()
                                    .Where(x => paymentIds.Contains(x.Id) && x.AnticipationId == null && x.Approved == true)
                                    .ToListAsync();

            if (payments.Count() == 0)
                return _resultService.GenerateFailedResult("None of the solicited payments are available for anticipation.", true);


            var entity = new AnticipationEntity
            {
                SolicitationDate = solicitationDate,
                SolicitedValue = 0,
            };

            await _dbContext.AddAsync(entity);

            await _dbContext.SaveChangesAsync();

            // sets the payment.solicitationId fields to the current anticipation id
            // needs to be done after the SaveChangesAsync so the id is set.
            foreach (var payment in payments)
            {
                payment.AnticipationId = entity.Id;
                entity.SolicitedValue += payment.LiquidValue * TaxRate;
            }

            var analysis = new AnticipationAnalysis
            {
                AnticipationId = entity.Id
            };

            await _dbContext.AddAsync(analysis);

            await _dbContext.SaveChangesAsync();

            await FillInternalEntities(entity);

            return _resultService.GenerateResult(entity);
        }

        public async Task<AnticipationEntity> Get(int id)
        {
            var entity = await _dbContext.Set<AnticipationEntity>()
                                    .AsQueryable()
                                    .Where(x => x.Id == id)
                                    .FirstOrDefaultAsync();

            if (entity == null)
                return entity;

            await FillInternalEntities(entity);

            return entity;
        }

        public async Task<List<AnticipationEntity>> Get(string status = null)
        {
            var anticipations = new List<AnticipationEntity>();
            if (status == null)
            {
                // retorna todas as antecipações
                anticipations = await _dbContext.Set<AnticipationEntity>()
                                .AsNoTracking()
                                .ToListAsync();

                await FillInternalEntities(anticipations);

                return anticipations;
            }

            var analysisQueryable = _dbContext.Set<AnticipationAnalysis>()
                            .AsNoTracking();

            switch (status)
            {
                case "pending":
                    analysisQueryable = analysisQueryable.Where(x => x.StartDate == null);
                    break;

                case "analyzing":
                    analysisQueryable = analysisQueryable.Where(x => x.StartDate != null && x.EndDate == null);
                    break;

                case "finished":
                    analysisQueryable = analysisQueryable.Where(x => x.EndDate != null);
                    break;
            }

            var analysis = await analysisQueryable.ToListAsync();

            foreach (var item in analysis)
            {
                anticipations.Add(await _dbContext.Set<AnticipationEntity>()
                                    .Where(x => x.Id == item.AnticipationId)
                                    .FirstOrDefaultAsync());
            }

            await FillInternalEntities(anticipations);

            return anticipations;
        }

        public async Task<AnticipationResult> StartAnalysis(int id, DateTime startDate)
        {
            var entity = await Get(id);

            if (entity == null)
                return _resultService.GenerateFailedResult($"No solicitation found for id = {id}");

            if (entity.Analysis.EndDate != null)
                return _resultService.GenerateFailedResult("Antecipation request is already closed.");

            if (entity.Analysis.StartDate != null)
                return _resultService.GenerateFailedResult("Analysis is already started", true);

            entity.Analysis.StartDate = startDate;

            await _dbContext.SaveChangesAsync();

            return _resultService.GenerateResult(entity);
        }

        public async Task<AnticipationResult> ResolvePaymentAnticipation(int anticipationId, List<int> paymentIds, bool approve, DateTime timeStamp)
        {
            var entity = await Get(anticipationId);

            if (entity == null)
                return _resultService.GenerateFailedResult($"No anticipation request found for id = {anticipationId}.");

            // if the analysis process isnt started, start it now
            if (entity.Analysis.StartDate == null)
                await StartAnalysis(anticipationId, timeStamp);

            var paymentsToResolve = entity.SolicitedPayments
                                    .AsQueryable()
                                    .Where(payment => paymentIds.Contains(payment.Id) && payment.Anticipated == null);

            if (paymentsToResolve.Count() == 0)
                return _resultService.GenerateFailedResult(
                            $"None of the solicited payments is available to be {(approve ? "approved" : "rejected")}.",
                            true
                            );

            foreach (var payment in paymentsToResolve)
            {
                payment.Anticipated = approve;
                var installments = await _dbContext.Set<PaymentInstallmentEntity>()
                                            .Where(x => x.PaymentId == payment.Id)
                                            .ToListAsync();

                payment.PaymentInstallments.AddRange(installments);

                if (approve)
                {
                    foreach (var installment in installments)
                    {
                        installment.AnticipatedValue = installment.LiquidValue * TaxRate;
                    }
                }
            }

            await _dbContext.SaveChangesAsync();

            var pendingPayments = await _dbContext.Set<PaymentEntity>()
                                        .Where(payment => payment.AnticipationId == anticipationId && payment.Anticipated == null)
                                        .ToListAsync();

            // Check if all payments in this solicitation were evaluated.
            if (pendingPayments.Count() == 0)
            {
                var deniedTrigger = false;
                var acceptedTrigger = false;
                entity.AnticipatedValue = 0;
                foreach (var payment in entity.SolicitedPayments)
                {
                    if (payment.Anticipated == true)
                    {
                        acceptedTrigger = true;

                        var installments = await _dbContext.Set<PaymentInstallmentEntity>()
                                           .Where(x => x.PaymentId == payment.Id)
                                           .ToListAsync();

                        foreach (var installment in installments)
                        {
                            installment.AnticipatedTranferDate = timeStamp;
                            installment.AnticipatedValue = installment.LiquidValue * TaxRate;
                            entity.AnticipatedValue += installment.LiquidValue * TaxRate;
                        }

                        payment.PaymentInstallments = installments;
                    }
                    else
                    {
                        deniedTrigger = true;
                    }
                }

                entity.Analysis.EndDate = timeStamp;

                if (deniedTrigger && !acceptedTrigger)
                    entity.Analysis.FinalStatus = "denied";

                else if (deniedTrigger && acceptedTrigger)
                    entity.Analysis.FinalStatus = "partially approved";

                else
                    entity.Analysis.FinalStatus = "approved";


                await _dbContext.SaveChangesAsync();
            }

            return _resultService.GenerateResult(entity);
        }

        private async Task FillInternalEntities(List<AnticipationEntity> entities)
        {
            foreach (var entity in entities)
            {
                await FillInternalEntities(entity);
            }
        }

        private async Task FillInternalEntities(AnticipationEntity entity)
        {
            entity.SolicitedPayments = await _dbContext.Set<PaymentEntity>()
                                                .Where(x => x.AnticipationId == entity.Id)
                                                .ToListAsync();

            entity.Analysis = await _dbContext.Set<AnticipationAnalysis>()
                                        .Where(x => x.AnticipationId == entity.Id)
                                              .FirstOrDefaultAsync();
        }
    }
}