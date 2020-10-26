namespace payment_api.Models.Result
{
    public class SolicitationProcessResult : IResult<AntecipationEntity>
    {
        public SolicitationProcessResult(string errorMessage)
        {
            Success = false;
            ErrorMessage = errorMessage;
        }

        public SolicitationProcessResult(AntecipationEntity value)
        {
            Success = true;
            Value = value;
        }

        public AntecipationEntity Value { get; set; }

        public bool Success { get; set; }

        public string ErrorMessage { get; set; }
    }
}