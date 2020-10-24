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

        public async Task<EntityCreationResult<PaymentEntity>> Create(PaymentEntity entity)
        {
            var result = new EntityCreationResult<PaymentEntity>(entity);

            if (!result.Success)
                return result;

            await _dbContext.AddAsync(result.Value);

            await _dbContext.SaveChangesAsync();

            return result;
        }

        public async Task<PaymentEntity> Get(int nsu)
            => await _dbContext.Set<PaymentEntity>()
                    .AsQueryable()
                    .Where(payment => payment.Nsu == nsu)
                    .FirstOrDefaultAsync();
    }
}