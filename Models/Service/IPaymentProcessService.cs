using System;
using System.Threading.Tasks;

namespace payment_api.Models.Service
{
    public interface IPaymentProcessService
    {
        Task<PaymentProcessResult> ProcessPayment(PaymentRequest request, DateTime transactionDate);
    }
}