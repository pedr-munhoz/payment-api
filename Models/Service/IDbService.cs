using System.Collections.Generic;
using System.Threading.Tasks;

namespace payment_api.Models.Service
{
    /// <summary>
    /// Service signature for dealing with data from the db.
    /// Supports any database implementation underneath.
    /// </summary>
    public interface IDbService
    {
        Task<PaymentEntity> Get(int nsu);

        Task<PaymentEntity> Create(PaymentEntity entity);

        Task<List<PaymentEntity>> GetAvailablePayments();
    }
}