namespace Claims.Utils;

public class Result<T>
{
    private Result(bool isSuccess, Error error, T? value = default)
    {
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }
        
        if (isSuccess && value == null)
        {
            throw new ArgumentException("Invalid success");
        }

        IsSuccess = isSuccess;
        Error = error;
        Value = value!;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }
    public T Value { get; }

    public static Result<T> Success(T value) => new(true, Error.None, value);

    public static Result<T> Failure(Error error) => new(false, error);
}

public sealed record Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);
}