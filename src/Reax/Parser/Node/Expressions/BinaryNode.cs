using Reax.Parser.Node.Interfaces;
using Reax.Runtime;

namespace Reax.Parser.Node.Expressions;

public record BinaryNode(
    ReaxNode Left, 
    ReaxNode Operator, 
    ReaxNode Right, 
    SourceLocation Location) : ExpressionNode(Location)
{
    public override string ToString()
    {
        return $"{Left} {Operator} {Right}";
    }
}
