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

    public override IReaxValue Evaluation(IReaxExecutionContext context)
    {
        var function = context.GetFunction(Identifier);
        var parameters = Parameter.Select(x => x.GetValue(context)).ToArray();
        var (success, error) = function.Invoke(parameters);
        if (error is not null)
            throw new ReturnErrorException(error);

        return success ?? throw new Exception();
    }

    public override string ToString()
    {
        return $"{Identifier}({string.Join(',', Parameter.Select(x => x.ToString()))});";
    }
}
