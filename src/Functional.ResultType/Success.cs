using System.Collections.Generic;

namespace Functional.ResultType;

public sealed class Success : ReasonBase, ISuccess
{
    private Success(string message, IDictionary<string, object>? metadata) : base(message, metadata)
    {
    }

    public static Success Create(string message, IDictionary<string, object>? metadata = null) =>
        new(message, metadata);
}