using System;
using System.Collections.Generic;

namespace Functional.ResultType;

public sealed class Error : IError, IEquatable<Error>
{
    public string Message { get; }
    public IDictionary<string, object> Metadata { get; }

    private Error(string message, IDictionary<string, object>? metadata)
    {
        Message = message;
        Metadata = metadata ?? new Dictionary<string, object>();
    }

    public static Error Create(string message, IDictionary<string, object>? metadata = null) => new(message, metadata);
    
    public bool Equals(Error? other)
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
        return obj.GetType() == GetType() && Equals((Error)obj);
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

    public static bool operator ==(Error? left, Error? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Error? left, Error? right)
    {
        return !(left == right);
    }
}