using System;
using System.Collections.Generic;
using System.Linq;

namespace Functional.ResultType;

public static class ResultIEnumerableExtensions
{
    public static IEnumerable<T> CollectSuccess<T>(this IEnumerable<Result<T>> list) => list.Collect(true);

    public static IEnumerable<T> CollectFails<T>(this IEnumerable<Result<T>> list) => list.Collect(false);

    private static IEnumerable<T> Collect<T>(this IEnumerable<Result<T>> list, bool isSuccess)
    {
        if (list == null)
        {
            throw new ArgumentNullException(nameof(list), "is null");
        }

        var collected = list.Where(w => w.IsSuccess == isSuccess).Select(s => s.Value);
        return collected;
    }
}