using System;

namespace Functional.ResultType;

public record Result<T>(bool IsSuccess, T Value, string Message = "")
{
    private static readonly Result<T> FailDefaultResult = new(false, default!, string.Empty);
    public Type Type => typeof(T);
    public static Result<T> Success(T value, string message = "") => new(true, value, message);
    public static Result<T> Fail(T value, string message = "") => new(false, value, message);

    public static bool TryParse(object? obj, Func<string> fnSuccessMessage, Func<string> fnFailMessage,
        out Result<T> result)
    {
        if (obj == null)
        {
            result = FailDefaultResult with { Message = fnFailMessage() };
            return false;
        }
        
        if (obj.GetType() != typeof(T))
        {
            result = FailDefaultResult with { Message = "Type mismatch" };
            return false;
        }

        result = Success((T)obj, fnSuccessMessage());
        return true;
    }

    public static bool TryParse(object? obj, out Result<T> result)
    {
        if (obj == null)
        {
            result = FailDefaultResult;
            return false;
        }

        if (obj.GetType() != typeof(T))
        {
            result = FailDefaultResult with { Message = "Type mismatch" };
            return false;
        }

        result = Success((T)obj, string.Empty);
        return true;
    }
}