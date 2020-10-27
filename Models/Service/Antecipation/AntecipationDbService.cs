using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using payment_api.Infrastructure.Database;
using payment_api.Models.Result;

namespace payment_api.Models.Service
{
    public class AntecipationDbService : IAntecipationDbService
    {
        private const double TaxRate = 0.962;
        private readonly ServerDbContext _dbContext;

        public AntecipationDbService(ServerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SolicitationProcessResult> Create(List<int> paymentIds, DateTime solicitationDate)
        {
            // Check if there's a open solicitation
            var openAnalysis = await _dbContext.Set<AntecipationAnalysis>()
                                        .Where(x => x.EndDate == null)
                                        .FirstOrDefaultAsync();
            if (openAnalysis != null)
                return new SolicitationProcessResult("Cannot open solicitation without finilizing the current one.", true);

            var payments = await _dbContext.Set<PaymentEntity>()
                                    .Where(x => paymentIds.Contains(x.Id) && x.SolicitationId == null)
                                    .ToListAsync();

            if (payments.Count() == 0)
                return new SolicitationProcessResult("None of the solicited payments are available for anticipation.", true);


            var entity = new AntecipationEntity
            {
                SolicitationDate = solicitationDate,
                SolicitedValue = 0,
            };

            await _dbContext.AddAsync(entity);

            await _dbContext.SaveChangesAsync();

            // sets the payment.solicitationId fields to the current antecipation id
            // needs to be done after the SaveChangesAsync so the id is set.
            foreach (var payment in payments)
            {
                payment.SolicitationId = entity.Id;
                entity.SolicitedValue += payment.LiquidValue * TaxRate;
            }

            var analysis = new AntecipationAnalysis
            {
                AntecipationId = entity.Id
            };

            await _dbContext.AddAsync(analysis);

            await _dbContext.SaveChangesAsync();

            await FillInternalEntities(entity);

            return new SolicitationProcessResult(entity);
        }

        public async Task<AntecipationEntity> Get(int id)
        {
            var entity = await _dbContext.Set<AntecipationEntity>()
                                    .AsQueryable()
                                    .Where(antecipation => antecipation.Id == id)
                                    .FirstOrDefaultAsync();

            if (entity == null)
                return entity;

            await FillInternalEntities(entity);

            return entity;
        }

        public async Task<List<AntecipationEntity>> Get(string status = null)
        {
            var antecipations = new List<AntecipationEntity>();
            if (status == null)
            {
                // retorna todas as antecipações
                antecipations = await _dbContext.Set<AntecipationEntity>()
                                .AsNoTracking()
                                .ToListAsync();

                await FillInternalEntities(antecipations);

                return antecipations;
            }

            var analysisQueryable = _dbContext.Set<AntecipationAnalysis>()
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
                antecipations.Add(await _dbContext.Set<AntecipationEntity>()
                                    .Where(x => x.Id == item.AntecipationId)
                                    .FirstOrDefaultAsync());
            }

            await FillInternalEntities(antecipations);

            return antecipations;
        }

        public async Task<SolicitationProcessResult> StartAnalysis(int id, DateTime startDate)
        {
            var entity = await Get(id);

            if (entity == null)
                return new SolicitationProcessResult($"No solicitation found for id = ${id}");

            if (entity.Analysis.StartDate != null)
                return new SolicitationProcessResult("Analysis is already started", true);

            entity.Analysis.StartDate = startDate;

            await _dbContext.SaveChangesAsync();

            return new SolicitationProcessResult(entity);
        }

        public async Task<SolicitationProcessResult> ResolvePaymentAntecipation(int antecipationId, List<int> paymentIds, bool approve)
        {
            var entity = await Get(antecipationId);

            if (entity == null)
                return new SolicitationProcessResult($"No antecipation request found for id = {antecipationId}.");

            // if the analysis process isnt started, start it now
            if (entity.Analysis.StartDate == null)
                await StartAnalysis(antecipationId, DateTime.Now);

            var paymentsToResolve = entity.SolicitedPayments
                                    .AsQueryable()
                                    .Where(payment => paymentIds.Contains(payment.Id) && payment.Anticipated == null);

            if (paymentsToResolve.Count() == 0)
                return new SolicitationProcessResult(
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
                                        .Where(payment => payment.SolicitationId == antecipationId && payment.Anticipated == null)
                                        .ToListAsync();

            // Check if all payments in this solicitation were evaluated.
            if (pendingPayments.Count() == 0)
            {
                var deniedTrigger = false;
                var acceptedTrigger = false;
                entity.AntecipatedValue = 0;
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
                            installment.AntecipatedTranfer = DateTime.Now;
                            installment.AnticipatedValue = installment.LiquidValue * TaxRate;
                            entity.AntecipatedValue += installment.LiquidValue * TaxRate;
                        }

                        payment.PaymentInstallments = installments;
                    }
                    else
                    {
                        deniedTrigger = true;
                    }
                }

                entity.Analysis.EndDate = DateTime.Now;

                if (deniedTrigger && !acceptedTrigger)
                    entity.Analysis.FinalStatus = "denied";

                else if (deniedTrigger && acceptedTrigger)
                    entity.Analysis.FinalStatus = "partially approved";

                else
                    entity.Analysis.FinalStatus = "approved";


                await _dbContext.SaveChangesAsync();
            }

            return new SolicitationProcessResult(entity);
        }

        private async Task FillInternalEntities(List<AntecipationEntity> entities)
        {
            foreach (var entity in entities)
            {
                await FillInternalEntities(entity);
            }
        }

        private async Task FillInternalEntities(AntecipationEntity entity)
        {
            entity.SolicitedPayments = await _dbContext.Set<PaymentEntity>()
                                                .Where(x => x.SolicitationId == entity.Id)
                                                .ToListAsync();

            entity.Analysis = await _dbContext.Set<AntecipationAnalysis>()
                                        .Where(x => x.AntecipationId == entity.Id)
                                              .FirstOrDefaultAsync();
        }
    }
}