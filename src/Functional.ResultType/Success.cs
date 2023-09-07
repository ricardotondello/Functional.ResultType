using System;
using System.Collections.Generic;

namespace Functional.ResultType;

public sealed class Success : ISuccess, IEquatable<Success>
{
    public string Message { get; }
    public IDictionary<string, object> Metadata { get; }

    private Success(string message, IDictionary<string, object>? metadata)
    {
        Message = message;
        Metadata = metadata ?? new Dictionary<string, object>();
    }

    public static Success Create(string message, IDictionary<string, object>? metadata = null) =>
        new(message, metadata);

    public bool Equals(Success? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (Message != other.Message)
        {
            return false;
        }

        if (Metadata.Count != other.Metadata.Count)
        {
            return false;
        }
        
        foreach (var kvp in Metadata)
        {
            if (!other.Metadata.TryGetValue(kvp.Key, out var otherValue) || kvp.Value != otherValue)
            {
                return false;
            }
        }

        return true;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Success)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = 17;
            hash = hash * 23 + Message.GetHashCode();
            foreach (var kvp in Metadata)
            {
                hash = hash * 23 + kvp.Key.GetHashCode();
                hash = hash * 23 + kvp.Value.GetHashCode();
            }
            return hash;
        }
    }

    public static bool operator ==(Success? left, Success? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Success? left, Success? right)
    {
        return !(left == right);
    }
}