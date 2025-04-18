using System;

namespace Reax.Parser.Node;

public record BindNode(string Identifier, ReaxNode[] Node) : ReaxNode
{
    public override string ToString()
    {
        return $"bind {Identifier} -> {{...}}";
    }
}
