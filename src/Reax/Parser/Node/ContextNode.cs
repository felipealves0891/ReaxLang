using Reax.Parser.Node.Interfaces;
using Reax.Runtime;

namespace Reax.Parser.Node;

public record ContextNode(
    ReaxNode[] Block, 
    SourceLocation Location) : ReaxNode(Location), IReaxContext
{
    public ReaxNode[] Nodes => Block;

    public override string ToString()
    {
        return "{...}";
    }
}
