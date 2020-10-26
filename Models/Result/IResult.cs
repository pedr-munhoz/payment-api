namespace payment_api.Models.Result
{
    public interface IResult<T>
    {
        T Value { get; }
        bool Success { get; }
    }
}