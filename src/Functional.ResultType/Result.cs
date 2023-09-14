using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Functional.ResultType;

public record Result<T>(bool IsSuccess, T Value, IEnumerable<IReason>? Reasons)
{
    private static readonly Result<T> FailDefaultResult = new(false, default!, Enumerable.Empty<IReason>());

    private static readonly Result<T> FailDefaultResultTypeMismatch =
        FailDefaultResult with { Reasons = new[] { Error.Create("Type mismatch") } };

    public Type Type => typeof(T);
    public bool HasErrors => GetErrors().Count > 0;
    public bool HasSuccesses => GetSuccesses().Count > 0;
    public static Result<T> Success(T value, IEnumerable<ISuccess>? successes = null) => new(true, value, successes);
    public static Result<T> Fail(T value, IEnumerable<IError>? errors = null) => new(false, value, errors);

    public IReadOnlyList<IError> GetErrors() =>
        new ReadOnlyCollection<IError>(Reasons?.OfType<IError>().ToArray() ?? Array.Empty<IError>());

    public IReadOnlyList<ISuccess> GetSuccesses() =>
        new ReadOnlyCollection<ISuccess>(Reasons?.OfType<ISuccess>().ToArray() ?? Array.Empty<ISuccess>());

    public static bool TryParse(object? obj, out Result<T> result)
    {
        switch (obj)
        {
            case null:
                result = FailDefaultResult;
                return false;
            case T castedValue:
                result = Success(castedValue);
                return true;
            default:
                result = FailDefaultResultTypeMismatch;
                return false;
        }
    }
}