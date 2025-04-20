using System;
using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record BindNode(
    string Identifier, 
    ReaxNode[] Node, 
    SourceLocation Location) : ReaxNode(Location)
{
    public ReaxNode[] Context => Node;

    public override string ToString()
    {
        return $"bind {Identifier} -> {{...}}";
    }
}
