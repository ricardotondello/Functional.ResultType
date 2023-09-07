using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Functional.ResultType;

public static class ResultExtensions
{
    public static Result<T> ToResultSuccess<T>(this object objSuccess) => Result<T>.Success((T)objSuccess);

    public static Result<T> ToResultSuccess<T>(this object objSuccess, IEnumerable<ISuccess> successes) => Result<T>.Success((T)objSuccess, successes);

    public static Result<T> ToResultFail<T>(this object objFail) => Result<T>.Fail((T)objFail);

    public static Result<T> ToResultFail<T>(this object objFail, IEnumerable<IError> errors) => Result<T>.Fail((T)objFail, errors);
    
    public static Result<T> FromReasons<T>(this object value, IEnumerable<IReason> reasons)
    {
        var reasonsList = reasons == null
            ? throw new ArgumentNullException(nameof(reasons), "is null")
            : reasons.ToArray();

        var errors = reasonsList.OfType<IError>().ToArray();
        var successes = reasonsList.OfType<ISuccess>().ToArray();
        
        if (errors.Length == 0 && successes.Length == 0)
        {
            throw new InvalidOperationException(
                $"The status cannot be defined! Reason: {nameof(reasons)} has no {nameof(ISuccess)} or {nameof(IError)} items.");
        }
        
        if (successes.Length > 0 && errors.Length > 0)
        {
            throw new InvalidOperationException(
                $"The status cannot be defined! Reason: {nameof(reasons)} has both {nameof(ISuccess)} and {nameof(IError)} items.");
        }

        return successes.Length > 0 
            ? Result<T>.Success((T)value, successes) 
            : Result<T>.Fail((T)value, errors);
    }
    
    public static Result<T> FromException<T>(this object value, Exception exception)
    {
        var errors = new[] { Error.Create(exception.ToString())};
        return Result<T>.Fail((T)value, errors);
    }

    #region Sync Functional Operations

    public static Result<T> WhenSuccess<T>(this Result<T> result, Action action)
    {
        if (result.IsSuccess)
        {
            action();
        }

        return result;
    }

    public static Result<T> WhenFail<T>(this Result<T> result, Action action)
    {
        if (!result.IsSuccess)
        {
            action();
        }

        return result;
    }

    public static Result<T> WhenMatch<T>(this Result<T> result, Action actionSuccess, Action actionFail)
    {
        if (result.IsSuccess)
        {
            actionSuccess();
        }
        else
        {
            actionFail();
        }

        return result;
    }

    public static Result<T> WhenSuccess<T>(this Result<T> result, Action<T> action)
    {
        if (result.IsSuccess)
        {
            action(result.Value);
        }

        return result;
    }

    public static Result<T> WhenFail<T>(this Result<T> result, Action<T> action)
    {
        if (!result.IsSuccess)
        {
            action(result.Value);
        }

        return result;
    }

    public static Result<T> WhenMatch<T>(this Result<T> result, Action<T> actionSuccess, Action<T> actionFail)
    {
        if (result.IsSuccess)
        {
            actionSuccess(result.Value);
        }
        else
        {
            actionFail(result.Value);
        }

        return result;
    }

    public static Result<T> WhenSuccess<T>(this Result<T> result, Action<Result<T>> action)
    {
        if (result.IsSuccess)
        {
            action(result);
        }

        return result;
    }

    public static Result<T> WhenFail<T>(this Result<T> result, Action<Result<T>> action)
    {
        if (!result.IsSuccess)
        {
            action(result);
        }

        return result;
    }

    public static Result<T> WhenMatch<T>(this Result<T> result, Action<Result<T>> actionSuccess,
        Action<Result<T>> actionFail)
    {
        if (result.IsSuccess)
        {
            actionSuccess(result);
        }
        else
        {
            actionFail(result);
        }

        return result;
    }

    public static Result<T> OnSuccess<T>(this Result<T> result, Func<Result<T>> fn) => result.IsSuccess ? fn() : result;

    public static Result<T> OnFail<T>(this Result<T> result, Func<Result<T>> fn) => !result.IsSuccess ? fn() : result;

    // ReSharper disable once UnusedParameter.Global
    public static Result<T> OnAny<T>(this Result<T> result, Func<Result<T>> fn) => fn();

    public static Result<T> OnSuccess<T>(this Result<T> result, Func<T, Result<T>> fn) =>
        result.IsSuccess ? fn(result.Value) : result;

    public static Result<T> OnFail<T>(this Result<T> result, Func<T, Result<T>> fn) =>
        !result.IsSuccess ? fn(result.Value) : result;

    public static Result<T> OnSuccess<T>(this Result<T> result, Func<Result<T>, Result<T>> fn) =>
        result.IsSuccess ? fn(result) : result;

    public static Result<T> OnFail<T>(this Result<T> result, Func<Result<T>, Result<T>> fn) =>
        !result.IsSuccess ? fn(result) : result;

    public static Result<T> OnAny<T>(this Result<T> result, Func<Result<T>, Result<T>> fn) => fn(result);

    #endregion

    #region Async Functional Operations

    public static Task<Result<T>> WhenSuccessAsync<T>(this Result<T> result, Action action)
    {
        if (result.IsSuccess)
        {
            action();
        }

        return Task.FromResult(result);
    }

    public static async Task<Result<T>> WhenSuccessAsync<T>(this Task<Result<T>> resultTask, Action action)
    {
        var result = await resultTask;
        if (result.IsSuccess)
        {
            action();
        }

        return result;
    }

    public static Task<Result<T>> WhenFailAsync<T>(this Result<T> result, Action action)
    {
        if (!result.IsSuccess)
        {
            action();
        }

        return Task.FromResult(result);
    }

    public static async Task<Result<T>> WhenFailAsync<T>(this Task<Result<T>> resultTask, Action action)
    {
        var result = await resultTask;
        if (!result.IsSuccess)
        {
            action();
        }

        return result;
    }

    public static Task<Result<T>> WhenMatchAsync<T>(this Result<T> result, Action actionSuccess, Action actionFail)
    {
        if (result.IsSuccess)
        {
            actionSuccess();
        }
        else
        {
            actionFail();
        }

        return Task.FromResult(result);
    }

    public static async Task<Result<T>> WhenMatchAsync<T>(this Task<Result<T>> resultTask, Action actionSuccess,
        Action actionFail)
    {
        var result = await resultTask;
        if (result.IsSuccess)
        {
            actionSuccess();
        }
        else
        {
            actionFail();
        }

        return result;
    }

    public static Task<Result<T>> WhenSuccessAsync<T>(this Result<T> result, Action<T> action)
    {
        if (result.IsSuccess)
        {
            action(result.Value);
        }

        return Task.FromResult(result);
    }

    public static async Task<Result<T>> WhenSuccessAsync<T>(this Task<Result<T>> resultTask, Action<T> action)
    {
        var result = await resultTask;
        if (result.IsSuccess)
        {
            action(result.Value);
        }

        return result;
    }

    public static Task<Result<T>> WhenFailAsync<T>(this Result<T> result, Action<T> action)
    {
        if (!result.IsSuccess)
        {
            action(result.Value);
        }

        return Task.FromResult(result);
    }

    public static async Task<Result<T>> WhenFailAsync<T>(this Task<Result<T>> resultTask, Action<T> action)
    {
        var result = await resultTask;
        if (!result.IsSuccess)
        {
            action(result.Value);
        }

        return result;
    }

    public static Task<Result<T>> WhenMatchAsync<T>(this Result<T> result, Action<T> actionSuccess,
        Action<T> actionFail)
    {
        if (result.IsSuccess)
        {
            actionSuccess(result.Value);
        }
        else
        {
            actionFail(result.Value);
        }

        return Task.FromResult(result);
    }

    public static async Task<Result<T>> WhenMatchAsync<T>(this Task<Result<T>> resultTask, Action<T> actionSuccess,
        Action<T> actionFail)
    {
        var result = await resultTask;
        if (result.IsSuccess)
        {
            actionSuccess(result.Value);
        }
        else
        {
            actionFail(result.Value);
        }

        return result;
    }

    public static Task<Result<T>> WhenSuccessAsync<T>(this Result<T> result, Action<Result<T>> action)
    {
        if (result.IsSuccess)
        {
            action(result);
        }

        return Task.FromResult(result);
    }

    public static async Task<Result<T>> WhenSuccessAsync<T>(this Task<Result<T>> resultTask, Action<Result<T>> action)
    {
        var result = await resultTask;
        if (result.IsSuccess)
        {
            action(result);
        }

        return result;
    }

    public static Task<Result<T>> WhenFailAsync<T>(this Result<T> result, Action<Result<T>> action)
    {
        if (!result.IsSuccess)
        {
            action(result);
        }

        return Task.FromResult(result);
    }

    public static async Task<Result<T>> WhenFailAsync<T>(this Task<Result<T>> resultTask, Action<Result<T>> action)
    {
        var result = await resultTask;
        if (!result.IsSuccess)
        {
            action(result);
        }

        return result;
    }

    public static Task<Result<T>> WhenMatchAsync<T>(this Result<T> result, Action<Result<T>> actionSuccess,
        Action<Result<T>> actionFail)
    {
        if (result.IsSuccess)
        {
            actionSuccess(result);
        }
        else
        {
            actionFail(result);
        }

        return Task.FromResult(result);
    }

    public static async Task<Result<T>> WhenMatchAsync<T>(this Task<Result<T>> resultTask,
        Action<Result<T>> actionSuccess, Action<Result<T>> actionFail)
    {
        var result = await resultTask;
        if (result.IsSuccess)
        {
            actionSuccess(result);
        }
        else
        {
            actionFail(result);
        }

        return result;
    }

    public static async Task<Result<T>> OnSuccessAsync<T>(this Result<T> result, Func<T, Task<Result<T>>> fn) =>
        result.IsSuccess ? await fn(result.Value) : result;

    public static async Task<Result<T>> OnFailAsync<T>(this Result<T> result, Func<T, Task<Result<T>>> fn) =>
        !result.IsSuccess ? await fn(result.Value) : result;

    public static async Task<Result<T>> OnSuccessAsync<T>(this Task<Result<T>> resultTask, Func<T, Task<Result<T>>> fn)
    {
        var result = await resultTask;
        return result.IsSuccess ? await fn(result.Value) : result;
    }

    public static async Task<Result<T>> OnFailAsync<T>(this Task<Result<T>> resultTask, Func<T, Task<Result<T>>> fn)
    {
        var result = await resultTask;
        return !result.IsSuccess ? await fn(result.Value) : result;
    }

    public static async Task<Result<T>> OnSuccessAsync<T>(this Result<T> result, Func<Task<Result<T>>> fn) =>
        result.IsSuccess ? await fn() : result;

    public static async Task<Result<T>> OnFailAsync<T>(this Result<T> result, Func<Task<Result<T>>> fn) =>
        !result.IsSuccess ? await fn() : result;

    // ReSharper disable once UnusedParameter.Global
    public static async Task<Result<T>> OnAnyAsync<T>(this Result<T> result, Func<Task<Result<T>>> fn) => await fn();

    public static async Task<Result<T>> OnSuccessAsync<T>(this Result<T> result, Func<Result<T>, Task<Result<T>>> fn) =>
        result.IsSuccess ? await fn(result) : result;

    public static async Task<Result<T>> OnFailAsync<T>(this Result<T> result, Func<Result<T>, Task<Result<T>>> fn) =>
        !result.IsSuccess ? await fn(result) : result;

    public static async Task<Result<T>> OnAnyAsync<T>(this Result<T> result, Func<Result<T>, Task<Result<T>>> fn) =>
        await fn(result);

    public static async Task<Result<T>> OnSuccessAsync<T>(this Task<Result<T>> resultTask, Func<Task<Result<T>>> fn)
    {
        var result = await resultTask;
        return result.IsSuccess ? await fn() : result;
    }

    public static async Task<Result<T>> OnFailAsync<T>(this Task<Result<T>> resultTask, Func<Task<Result<T>>> fn)
    {
        var result = await resultTask;
        return !result.IsSuccess ? await fn() : result;
    }

    // ReSharper disable once UnusedParameter.Global
    public static async Task<Result<T>> OnAnyAsync<T>(this Task<Result<T>> resultTask, Func<Task<Result<T>>> fn) =>
        await fn();

    public static async Task<Result<T>> OnSuccessAsync<T>(this Task<Result<T>> resultTask,
        Func<Result<T>, Task<Result<T>>> fn)
    {
        var result = await resultTask;
        return result.IsSuccess ? await fn(result) : result;
    }

    public static async Task<Result<T>> OnFailAsync<T>(this Task<Result<T>> resultTask,
        Func<Result<T>, Task<Result<T>>> fn)
    {
        var result = await resultTask;
        return !result.IsSuccess ? await fn(result) : result;
    }

    public static async Task<Result<T>> OnAnyAsync<T>(this Task<Result<T>> resultTask,
        Func<Result<T>, Task<Result<T>>> fn)
    {
        var result = await resultTask;
        return await fn(result);
    }

    #endregion
}