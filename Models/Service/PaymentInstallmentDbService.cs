using payment_api.Infrastructure.Database;

namespace payment_api.Models.Service
{
    public class PaymentInstallmentDbService : BaseDbService<PaymentInstallmentEntity>, IPaymentInstallmentDbService
    {
        public PaymentInstallmentDbService(ServerDbContext dbContext) : base(dbContext)
        {
        }
    }
}