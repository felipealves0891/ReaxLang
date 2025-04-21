using System;
using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record BindNode(
    string Identifier, 
    ReaxNode Node,
    DataTypeNode DataType, 
    SourceLocation Location) : ReaxNode(Location)
{
    public override string ToString()
    {
        return $"bind {Identifier}: {DataType} -> {{...}}";
    }
}
