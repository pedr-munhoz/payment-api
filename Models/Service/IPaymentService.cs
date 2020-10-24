using System.Collections.Generic;
using System.Threading.Tasks;

namespace payment_api.Models.Service
{
    public interface IPaymentService
    {
        Task<PaymentEntity> Get(int nsu);

        Task<EntityCreationResult<PaymentEntity>> Create(PaymentEntity entity);
    }
}