using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record ForNode(
    ReaxNode Declaration, 
    ReaxNode Condition, 
    ContextNode Context, 
    SourceLocation Location) : ReaxNode(Location), IReaxContext
{
    public ReaxNode[] Branchs => Context.Branchs;

    public override string ToString()
    {
        return $"for {Declaration} to {Condition} {{...}}";
    }
}
