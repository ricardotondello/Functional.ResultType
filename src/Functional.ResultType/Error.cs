using System.Collections.Generic;

namespace Functional.ResultType;

public sealed class Error : ReasonBase, IError
{
    private Error(string message, IDictionary<string, object>? metadata) : base(message, metadata)
    {
    }

    public static Error Create(string message, IDictionary<string, object>? metadata = null) => new(message, metadata);
}