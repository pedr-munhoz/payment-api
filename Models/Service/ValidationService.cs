using payment_api.Models.Result;

namespace payment_api.Models.Service
{
    public class ResultService : IResultService
    {
        public PaymentProcessResult GenerateResult(PaymentEntity payment, bool approved)
            => new PaymentProcessResult(payment, approved);

        public PaymentProcessResult GenerateFailedResult()
            => new PaymentProcessResult(false);

    }
}