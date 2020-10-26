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

        private readonly IPaymentDbService _paymentDbService;

        public AntecipationDbService(ServerDbContext dbContext, IPaymentDbService paymentDbService)
        {
            _dbContext = dbContext;
            _paymentDbService = paymentDbService;
        }

        public async Task<SolicitationProcessResult> Create(List<int> paymentIds, DateTime solicitationDate)
        {
            // Check if there's a open solicitation
            var openAnalysis = await _dbContext.Set<AntecipationAnalysis>()
                                        .Where(x => x.EndDate == null)
                                        .FirstOrDefaultAsync();
            if (openAnalysis != null)
            {
                return new SolicitationProcessResult("Cannot open solicitation without finilizing the current one.");
            }



            var entity = new AntecipationEntity
            {
                SolicitationDate = solicitationDate,
                SolicitedValue = 0,
            };

            await _dbContext.AddAsync(entity);

            await _dbContext.SaveChangesAsync();



            // sets the payment.solicitationId fields to the current antecipation id
            // needs to be done after the SaveChangesAsync so the id is set.
            foreach (var paymentId in paymentIds)
            {
                var payment = await _dbContext.Set<PaymentEntity>()
                                            .Where(x => x.Id == paymentId)
                                            .FirstOrDefaultAsync();

                if (payment != null)
                {
                    if (payment.SolicitationId == null)
                    {
                        payment.SolicitationId = entity.Id;
                        entity.SolicitedValue += payment.LiquidValue * TaxRate;
                        entity.SolicitedPayments.Add(payment);
                    }
                }
            }

            var analysis = new AntecipationAnalysis
            {
                AntecipationId = entity.Id
            };

            await _dbContext.AddAsync(analysis);

            await _dbContext.SaveChangesAsync();

            entity.Analysis = analysis;

            return new SolicitationProcessResult(entity);
        }

        public async Task<AntecipationEntity> Get(int id)
        {
            var entity = await _dbContext.Set<AntecipationEntity>()
                                    .AsQueryable()
                                    .Where(antecipation => antecipation.Id == id)
                                    .FirstOrDefaultAsync();

            entity.SolicitedPayments = await _dbContext.Set<PaymentEntity>()
                                                .Where(x => x.SolicitationId == id)
                                                .ToListAsync();

            return entity;
        }

        public async Task<List<AntecipationEntity>> Get(string status = null)
        {
            var analysis = new List<AntecipationAnalysis>();
            var antecipations = new List<AntecipationEntity>();
            if (status == "pending")
            {
                analysis = await _dbContext.Set<AntecipationAnalysis>()
                            .AsNoTracking()
                            .Where(x => x.StartDate == null)
                            .ToListAsync();
            }

            else if (status == "analyzing")
            {
                analysis = await _dbContext.Set<AntecipationAnalysis>()
                            .AsNoTracking()
                            .Where(x => x.StartDate != null && x.EndDate == null)
                            .ToListAsync();
            }

            else if (status == "finished")
            {
                analysis = await _dbContext.Set<AntecipationAnalysis>()
                            .AsNoTracking()
                            .Where(x => x.EndDate != null)
                            .ToListAsync();
            }

            else
            {
                antecipations = await _dbContext.Set<AntecipationEntity>()
                                .AsNoTracking()
                                .ToListAsync();
            }

            foreach (var item in analysis)
            {
                antecipations.Add(await _dbContext.Set<AntecipationEntity>()
                                    .Where(x => x.Id == item.AntecipationId)
                                    .FirstOrDefaultAsync());


            }

            foreach (var entity in antecipations)
            {
                entity.SolicitedPayments = await _dbContext.Set<PaymentEntity>()
                                                .Where(x => x.SolicitationId == entity.Id)
                                                .ToListAsync();
            }
            return antecipations;
        }

        public async Task<SolicitationProcessResult> StartAnalysis(int id, DateTime startDate)
        {
            var entity = await Get(id);

            if (entity == null)
                return new SolicitationProcessResult($"No solicitation found for id = ${id}");

            var analysis = await _dbContext.Set<AntecipationAnalysis>()
                                        .Where(x => x.AntecipationId == id)
                                        .FirstOrDefaultAsync();

            if (analysis.StartDate != null)
                return new SolicitationProcessResult("Analysis is already started");

            analysis.StartDate = startDate;

            await _dbContext.SaveChangesAsync();

            entity.Analysis = analysis;

            return new SolicitationProcessResult(entity);
        }

        public async Task<AntecipationEntity> ResolvePaymentAntecipation(int antecipationId, List<int> paymentIds, bool approve)
        {
            var entity = await Get(antecipationId);

            var payments = entity.SolicitedPayments.AsQueryable()
                                        .Where(payment => paymentIds.Contains(payment.Id));

            foreach (var payment in payments)
            {
                if (payment.Anticipated == null)
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

                var analysis = await _dbContext.Set<AntecipationAnalysis>()
                                    .Where(x => x.AntecipationId == antecipationId)
                                    .FirstOrDefaultAsync();

                entity.Analysis = analysis;

                analysis.EndDate = DateTime.Now;

                if (deniedTrigger && !acceptedTrigger)
                    analysis.FinalStatus = "denied";

                else if (deniedTrigger && acceptedTrigger)
                    analysis.FinalStatus = "partially approved";

                else
                    analysis.FinalStatus = "approved";


                await _dbContext.SaveChangesAsync();
            }

            return entity;
        }
    }
}