namespace payment_api.Models.Service
{
    public class ValidationService : IValidationService
    {
        public PaymentProcessResult Validate(EntityCreationResult<PaymentEntity> result, bool approved)
            => new PaymentProcessResult(result, approved);

        public PaymentProcessResult Validate(PaymentEntity payment, bool approved)
            => new PaymentProcessResult(payment, approved);

    }
}