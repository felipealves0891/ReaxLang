using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record WhileNode(
    ReaxNode condition, 
    ContextNode Block, 
    SourceLocation Location) : ReaxNode(Location)
{
    public override string ToString()
    {
        return $"while {condition} {{...}}";
    }
}
