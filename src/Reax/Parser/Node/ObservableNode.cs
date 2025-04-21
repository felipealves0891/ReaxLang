using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record ObservableNode(ReaxNode Var, ReaxNode Block, BinaryNode? Condition, 
    SourceLocation Location) : ReaxNode(Location), IReaxContext
{
    public ReaxNode[] Nodes 
    {
        get 
        {
            if(Block is ContextNode node)
                return node.Block;
            else if(Block is IReaxContext context)
                return context.Nodes;
            else
                return [Block];
        }
    }

    public override string ToString()
    {
        return $"on {Var} {{...}}";
    }
}
