using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TestMongoDB.Models.Helpers;

public class Result<T>
{
    private Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    private Result(bool isSuccess, Error error, T value) : this(isSuccess, error)
    {
        Value = value;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }
    public T? Value { get; private set; }

    public static Result<T> Success(T value) => new(true, Error.None, value);
    public static Result<T> Success() => new(true, Error.None);
    public static Result<T> Failure(Error error) => new(false, error);
}
