namespace Functional.ResultType;

public record Result<T>(bool IsSuccess, T Value, string Message = "")
{
    public static Result<T> Success(T value, string message = "") => new(true, value, message);
    public static Result<T> Fail(T value, string message = "") => new(false, value, message);
}