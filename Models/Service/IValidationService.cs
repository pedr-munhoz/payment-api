using payment_api.Models.Result;

namespace payment_api.Models.Service
{
    public interface IResultService
    {
        PaymentProcessResult GenerateResult(PaymentEntity payment, bool approved);

        PaymentProcessResult GenerateFailedResult(PaymentEntity payment, string eerrorMessage);
    }
}