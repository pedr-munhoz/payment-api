using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using payment_api.Infrastructure.Database;

namespace payment_api.Models.Service
{
    public class AntecipationDbService : IAntecipationDbService
    {
        private readonly ServerDbContext _dbContext;

        private readonly IPaymentDbService _paymentDbService;

        public AntecipationDbService(ServerDbContext dbContext, IPaymentDbService paymentDbService)
        {
            _dbContext = dbContext;
            _paymentDbService = paymentDbService;
        }

        public async Task<AntecipationEntity> Create(List<int> paymentIds, DateTime solicitationDate)
        {
            var entity = new AntecipationEntity
            {
                SolicitationDate = solicitationDate,
                SolicitedValue = 0,
                Analysis = new AntecipationAnalysis(),
            };

            await _dbContext.AddAsync(entity);

            await _dbContext.SaveChangesAsync();

            foreach (var id in paymentIds)
            {
                var payment = await _dbContext.Set<PaymentEntity>()
                                            .Where(x => x.Id == id)
                                            .FirstOrDefaultAsync();

                if (payment != null)
                    payment.SolicitationId = entity.Id;

                entity.SolicitedValue += payment.LiquidValue;

                entity.SolicitedPayments.Add(payment);
            }

            await _dbContext.SaveChangesAsync();

            return entity;
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
            List<AntecipationEntity> antecipations;
            if (status == "pending")
            {
                antecipations = await _dbContext.Set<AntecipationEntity>()
                                    .AsNoTracking()
                                    .AsQueryable()
                                    .Where(antecipation => antecipation.Analysis.StartDate == null)
                                    .ToListAsync();

                return antecipations;
            }

            else if (status == "analyzing")
            {
                antecipations = await _dbContext.Set<AntecipationEntity>()
                                    .AsNoTracking()
                                    .AsQueryable()
                                    .Where(antecipation => antecipation.Analysis.StartDate != null && antecipation.Analysis.EndDate == null)
                                    .ToListAsync();
            }

            else if (status == "finished")
            {
                antecipations = await _dbContext.Set<AntecipationEntity>()
                                    .AsNoTracking()
                                    .AsQueryable()
                                    .Where(antecipation => antecipation.Analysis.EndDate != null)
                                    .ToListAsync();
            }

            else
            {
                antecipations = await _dbContext.Set<AntecipationEntity>()
                                .AsNoTracking()
                                .ToListAsync();
            }


            foreach (var entity in antecipations)
            {
                entity.SolicitedPayments = await _dbContext.Set<PaymentEntity>()
                                                .Where(x => x.SolicitationId == entity.Id)
                                                .ToListAsync();
            }
            return antecipations;
        }

        public async Task<AntecipationEntity> StartAnalysis(int id, DateTime startDate)
        {
            var entity = await Get(id);

            if (entity == null)
                return null;

            entity.Analysis.StartDate = startDate;

            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public async Task<AntecipationEntity> ResolvePaymentAntecipation(int antecipationId, List<int> paymentIds, bool approve)
        {
            var entity = await Get(antecipationId);

            var payments = entity.SolicitedPayments.AsQueryable()
                                        .Where(payment => paymentIds.Contains(payment.Id));

            foreach (var payment in payments)
            {
                payment.Anticipated = approve;
            }

            await _dbContext.SaveChangesAsync();

            return entity;
        }
    }
}