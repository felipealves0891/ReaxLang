using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node.Expressions;

public record ReturnSuccessNode(
    ReaxNode Expression, 
    SourceLocation Location) : ExpressionNode(Location)
{
    public override string ToString()
    {
        return $"return success {Expression}";
    }
}
