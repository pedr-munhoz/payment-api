using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using payment_api.Infrastructure.Database;

namespace payment_api.Models.Service
{
    /// <summary>
    /// Class with a PostgreSQL implementation to satisfy the <see cref="PaymentDbService"/> requirements.
    /// </summary>
    public class PaymentDbService : IPaymentDbService
    {
        /// <summary>
        /// PostgreSQL database context.
        /// </summary>
        private readonly ServerDbContext _dbContext;

        public PaymentDbService(ServerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PaymentEntity> Create(PaymentEntity entity)
        {
            await _dbContext.AddAsync(entity);

            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public async Task<PaymentEntity> Get(int id)
        {
            var entity = await _dbContext.Set<PaymentEntity>()
                    .Where(payment => payment.Id == id)
                    .FirstOrDefaultAsync();

            entity.PaymentInstallments = await _dbContext.Set<PaymentInstallmentEntity>()
                                                    .Where(x => x.PaymentId == id)
                                                    .ToListAsync();

            return entity;
        }

        public async Task<List<PaymentEntity>> GetAvailablePayments()
        {
            var entities = await _dbContext.Set<PaymentEntity>()
                    .Where(payment => payment.Anticipated == null && payment.Approved)
                    .ToListAsync();

            foreach (var entity in entities)
            {
                entity.PaymentInstallments = await _dbContext.Set<PaymentInstallmentEntity>()
                                                    .Where(x => x.PaymentId == entity.Id)
                                                    .ToListAsync();
            }

            return entities;
        }
    }
}