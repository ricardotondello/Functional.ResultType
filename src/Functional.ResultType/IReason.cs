using System.Collections.Generic;

namespace Functional.ResultType;

public interface IReason
{
    string Message { get; }

    IDictionary<string, object> Metadata { get; }
}