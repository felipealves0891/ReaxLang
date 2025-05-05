using System;
using Reax.Parser;

namespace Reax.Semantic.Contexts;

public class Reference
{
    public Reference(string identifier, Type type, bool isAssignment, SourceLocation location)
    {
        Identifier = identifier;
        Type = type;
        IsAssignment = isAssignment;
        Location = location;
    }

    public string Identifier { get; }
    public Type Type { get; }
    public bool IsAssignment { get; }
    public SourceLocation Location { get; }

    public override bool Equals(object? obj)
    {
        return obj is Reference reference
            && reference.Identifier == Identifier;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Identifier);
    }

    public override string ToString()
    {
        return $"({Type.Name}:{Location.Line}){Identifier}";
    }
}
