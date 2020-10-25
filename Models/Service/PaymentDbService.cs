using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using payment_api.Infrastructure.Database;

namespace payment_api.Models.Service
{
    /// <summary>
    /// Class with a PostgreSQL implementation to satisfy the <see cref="IPaymentDbService"/> requirements.
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

        public async Task<PaymentEntity> Get(int nsu)
            => await _dbContext.Set<PaymentEntity>()
                    .AsQueryable()
                    .Where(payment => payment.Id == nsu)
                    .FirstOrDefaultAsync();

        public async Task<List<PaymentEntity>> GetAvailablePayments()
            => await _dbContext.Set<PaymentEntity>()
                    .AsQueryable()
                    .Where(payment => payment.Anticipated == null)
                    .ToListAsync();
    }
}