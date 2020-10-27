using payment_api.Models.Result;

namespace payment_api.Models.Service
{
    public interface IResultService
    {
        PaymentProcessResult GenerateResult(PaymentEntity payment, bool approved);

        AnticipationResult GenerateResult(AntecipationEntity anticipation);

        PaymentProcessResult GenerateFailedResult(PaymentEntity payment, string eerrorMessage);

        AnticipationResult GenerateFailedResult(string errorMessage, bool unprocessableEntity = false);
    }
}