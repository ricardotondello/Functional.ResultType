using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Functional.ResultType;

public record Result<T>
{
    private static readonly Result<T> FailDefaultResult = new(false, default!, Enumerable.Empty<IReason>());

    private static readonly Result<T> FailDefaultResultTypeMismatch =
        new(false, default!, new[] { Error.Create("Type mismatch") });

    public bool IsSuccess { get; }
    public T Value { get; }
    private IEnumerable<IReason>? Reasons { get; }
    public Type Type => typeof(T);
    public bool HasErrors { get; }
    public bool HasSuccesses { get; }
    public IReadOnlyList<IError> Errors { get; }
    public IReadOnlyList<ISuccess> Successes { get; }

    private Result(bool isSuccess, T value, IEnumerable<IReason>? reasons)
    {
        IsSuccess = isSuccess;
        Value = value;
        Reasons = reasons ?? Enumerable.Empty<IReason>();

        var enumerable = Reasons as IReason[] ?? Reasons.ToArray();
        Errors = new ReadOnlyCollection<IError>(enumerable.OfType<IError>().ToArray());
        Successes = new ReadOnlyCollection<ISuccess>(enumerable.OfType<ISuccess>().ToArray());
        HasErrors = Errors.Count > 0;
        HasSuccesses = Successes.Count > 0;
    }

    public static Result<T> Success(T value, IEnumerable<ISuccess>? successes = null) => new(true, value, successes);
    public static Result<T> Fail(T value, IEnumerable<IError>? errors = null) => new(false, value, errors);

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