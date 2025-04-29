using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record ReturnSuccessNode(
    ReaxNode Expression, 
    SourceLocation Location) : ReaxNode(Location)
{
    public override string ToString()
    {
        return $"return success {Expression}";
    }
}
