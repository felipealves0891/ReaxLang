using Reax.Parser.Node.Interfaces;
using Reax.Runtime;

namespace Reax.Parser.Node.Expressions;

public record ReturnErrorNode(
    ReaxNode Expression, 
    SourceLocation Location) : ExpressionNode(Location)
{
    public override string ToString()
    {
        return $"return error {Expression}";
    }
}
