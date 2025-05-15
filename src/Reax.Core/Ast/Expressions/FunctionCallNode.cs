using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;

namespace Reax.Core.Ast.Expressions;

[ExcludeFromCodeCoverage]
public record FunctionCallNode(
    string Identifier, 
    ReaxNode[] Parameter, 
    SourceLocation Location) : ExpressionNode(Location)
{
    public override IReaxNode[] Children => Parameter;

    public override LiteralNode Evaluation(IReaxExecutionContext context)
    {
        var function = context.GetFunction(Identifier);
        var parameters = Parameter.Select(x => x.GetValue(context)).ToArray();
        var (success, error) = function.Invoke(parameters);
        return success ?? error ?? throw new Exception();
    }

    public override string ToString()
    {
        return $"{Identifier}({string.Join(',', Parameter.Select(x => x.ToString()))});";
    }
}
