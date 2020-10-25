namespace payment_api.Models.Service
{
    public interface IValidationService
    {
        PaymentProcessResult Validate(EntityCreationResult<PaymentEntity> result, bool approved);
        PaymentProcessResult Validate(PaymentEntity payment, bool approved);
    }
}