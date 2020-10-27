using payment_api.Models.Result;

namespace payment_api.Models.Service
{
    public class ResultService : IResultService
    {
        public PaymentProcessResult GenerateResult(PaymentEntity payment, bool approved)
            => new PaymentProcessResult(payment, approved);

        public PaymentProcessResult GenerateFailedResult(PaymentEntity payment, string errorMessage)
            => new PaymentProcessResult(payment, false, errorMessage);

        public AnticipationResult GenerateResult(AntecipationEntity anticipation)
            => new AnticipationResult(anticipation);

        public AnticipationResult GenerateFailedResult(string errorMessage, bool unprocessableEntity = false)
            => new AnticipationResult(errorMessage, unprocessableEntity);
    }
}