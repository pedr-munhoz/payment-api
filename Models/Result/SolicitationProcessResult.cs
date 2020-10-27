namespace payment_api.Models.Result
{
    public class SolicitationProcessResult : IResult<AntecipationEntity>
    {
        public SolicitationProcessResult(string errorMessage, bool unprocessableEntity = false)
        {
            Success = false;
            ErrorMessage = errorMessage;
            UnprocessableEntity = unprocessableEntity;
        }

        public SolicitationProcessResult(AntecipationEntity value)
        {
            Success = true;
            Value = value;
        }

        public AntecipationEntity Value { get; set; }

        public bool UnprocessableEntity { get; set; }

        public bool Success { get; set; }

        public string ErrorMessage { get; set; }
    }
}