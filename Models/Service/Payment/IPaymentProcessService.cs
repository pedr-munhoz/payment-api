using System;
using System.Threading.Tasks;
using payment_api.Models.Result;

namespace payment_api.Models.Service
{
    public interface IPaymentProcessService
    {
        Task<PaymentProcessResult> ProcessPayment(PaymentRequest request, DateTime transactionDate);
    }
}