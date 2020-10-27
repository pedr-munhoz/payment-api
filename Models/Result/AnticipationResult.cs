namespace payment_api.Models.Result
{
    public class AnticipationResult : IResult<AnticipationEntity>
    {
        public AnticipationResult(string errorMessage, bool unprocessableEntity = false)
        {
            Success = false;
            ErrorMessage = errorMessage;
            UnprocessableEntity = unprocessableEntity;
        }

        public AnticipationResult(AnticipationEntity value)
        {
            Success = true;
            Value = value;
        }

        public AnticipationEntity Value { get; set; }

        public bool UnprocessableEntity { get; set; }

        public bool Success { get; set; }

        public string ErrorMessage { get; set; }
    }
}