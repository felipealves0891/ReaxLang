using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record ForNode(
    ReaxNode Declaration, 
    ReaxNode Condition, 
    ContextNode Context, 
    SourceLocation Location) : ReaxNode(Location)
{

    public override string ToString()
    {
        return $"for {Declaration} to {Condition} {{...}}";
    }
}
