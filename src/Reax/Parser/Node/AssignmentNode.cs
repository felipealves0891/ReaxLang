using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record AssignmentNode(
    string Identifier, 
    ReaxNode Assigned, 
    SourceLocation Location) : ReaxNode(Location), IReaxContext
{
    public ReaxNode[] Branchs 
    {
        get
        {
            if(Assigned is IReaxContext context)
                return context.Branchs;
            else 
                return [Assigned];
        }
    }

    public override string ToString()
    {
        return $"{Identifier} = {Assigned};";
    }
}
