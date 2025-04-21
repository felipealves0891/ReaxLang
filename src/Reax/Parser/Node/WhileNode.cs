using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record WhileNode(ReaxNode condition, ReaxNode Block, 
    SourceLocation Location) : ReaxNode(Location), IReaxContext
{
    public ReaxNode[] Nodes
    {
        get
        {
            if(Block is IReaxContext context)
                return context.Nodes;
            else 
                return [Block];
        }
    }

    public override string ToString()
    {
        return $"while {condition} {{...}}";
    }
}
