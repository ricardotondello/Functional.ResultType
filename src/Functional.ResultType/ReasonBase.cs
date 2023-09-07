using System.Collections.Generic;

namespace Functional.ResultType;

public abstract class ReasonBase : IReason
{
    public string Message { get; }
    public IDictionary<string, object> Metadata { get; }

    protected ReasonBase(string message, IDictionary<string, object>? metadata)
    {
        Message = message;
        Metadata = metadata ?? new Dictionary<string, object>();
    }
}