using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record AssignmentNode(
    string Identifier, 
    ReaxNode Assigned, 
    SourceLocation Location) : ReaxNode(Location), IReaxContext
{
    public ReaxNode[] Nodes
    {
        get
        {
            if(Assigned is IReaxContext context)
                return context.Nodes;
            else
                return [Assigned];
        }
    }

    public override string ToString()
    {
        return $"{Identifier} = {Assigned};";
    }
}
