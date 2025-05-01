using Reax.Parser.Node.Interfaces;
using Reax.Runtime;

namespace Reax.Parser.Node.Expressions;

public record FunctionCallNode(
    string Identifier, 
    ReaxNode[] Parameter, 
    SourceLocation Location) : ExpressionNode(Location)
{
    public override string ToString()
    {
        return $"{Identifier}({string.Join(',', Parameter.Select(x => x.ToString()))});";
    }
}
