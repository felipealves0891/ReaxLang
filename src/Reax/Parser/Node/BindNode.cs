using System;
using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record BindNode(
    IdentifierNode Identifier, 
    ContextNode Node,
    DataTypeNode DataType, 
    SourceLocation Location) : ReaxNode(Location)
{
    public override string ToString()
    {
        return $"bind {Identifier}: {DataType} -> {{...}}";
    }
}
