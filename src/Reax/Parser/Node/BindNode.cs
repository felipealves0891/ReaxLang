using System;
using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record BindNode(
    string Identifier, 
    ReaxNode Node, 
    SourceLocation Location) : ReaxNode(Location), IReaxContext
{
    public ReaxNode[] Nodes
    {
        get
        {
            if(Node is IReaxContext context)
                return context.Nodes;
            else 
                return [Node];
        }
    }

    public override string ToString()
    {
        return $"bind {Identifier} -> {{...}}";
    }
}
