using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Functional.ResultType;

public record Result<T>(bool IsSuccess, T Value, IEnumerable<IReason>? Reasons)
{
    private static readonly Result<T> FailDefaultResult = new(false, default!, Enumerable.Empty<IReason>());
    public Type Type => typeof(T);
    public bool HasErrors => GetErrors().Count > 0;
    public bool HasSuccesses => GetSuccesses().Count > 0;
    public static Result<T> Success(T value, IEnumerable<ISuccess>? successes = null) => new(true, value, successes);
    public static Result<T> Fail(T value, IEnumerable<IError>? errors = null) => new(false, value, errors);
    public IReadOnlyList<IError> GetErrors() => new ReadOnlyCollection<IError>(Reasons?.OfType<IError>().ToArray() ?? Array.Empty<IError>());
    public IReadOnlyList<ISuccess> GetSuccesses() => new ReadOnlyCollection<ISuccess>(Reasons?.OfType<ISuccess>().ToArray() ?? Array.Empty<ISuccess>());
    public static bool TryParse(object? obj, out Result<T> result)
    {
        if (obj == null)
        {
            result = FailDefaultResult;
            return false;
        }

        if (IsTypeMismatch(obj, out var mismatchResult))
        {
            result = mismatchResult!;
            return false;
        }

        result = Success((T)obj);
        return true;
    }

    private static bool IsTypeMismatch(object? obj, out Result<T>? result)
    {
        if (obj!.GetType() != typeof(T))
        {
            result = FailDefaultResult with { Reasons = new[] { Error.Create("Type mismatch") } };
            return true;
        }

        result = null;
        return false;
    }
}