namespace payment_api.Models.Result
{
    public class PaymentProcessResult : IResult<PaymentEntity>
    {
        public PaymentProcessResult(PaymentEntity value, bool success)
        {
            Value = value;
            Success = success;
        }
        public PaymentProcessResult(PaymentEntity value, bool success, string errorMessage)
        {
            Value = value;
            Success = success;
            ErrorMessage = errorMessage;
        }

        public PaymentProcessResult(bool success)
        {
            Success = success;
        }

        public PaymentEntity Value { get; set; }

        public bool Success { get; set; }

        public string ErrorMessage { get; set; }
    }
}