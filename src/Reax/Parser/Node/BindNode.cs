using System;
using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record BindNode(
    IdentifierNode Identifier, 
    ContextNode Node,
    DataType Type, 
    SourceLocation Location) : ReaxNode(Location)
{
    public override string ToString()
    {
        return $"bind {Identifier}: {Type} -> {{...}}";
    }
}
