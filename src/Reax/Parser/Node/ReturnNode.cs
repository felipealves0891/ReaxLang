using Reax.Parser.Node.Interfaces;
using Reax.Runtime;

namespace Reax.Parser.Node;

public record ReturnNode(
    ReaxNode Expression, 
    SourceLocation Location) : ReaxNode(Location), IReaxContext
{
    public ReaxNode[] Nodes
    {
        get
        {
            if(Expression is IReaxContext context)
                return context.Nodes;
            else 
                return [Expression];
        }
    }

    public override string ToString()
    {
        return $"return {Expression}";
    }
}
