namespace payment_api.Models
{
    public class PaymentProcessResult
    {
        public PaymentProcessResult(PaymentEntity value, bool approved)
        {
            CreationResult = new EntityCreationResult<PaymentEntity>(value);
            Approved = approved;
        }

        public PaymentProcessResult(EntityCreationResult<PaymentEntity> result, bool approved)
        {
            CreationResult = result;
            Approved = approved;
        }

        public EntityCreationResult<PaymentEntity> CreationResult { get; set; }
        public bool Approved { get; set; }
    }
}