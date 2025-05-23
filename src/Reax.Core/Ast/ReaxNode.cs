using Reax.Core.Locations;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;
using Reax.Parser.Node;
using Reax.Core.Ast.Objects;

namespace Reax.Core.Ast;

public abstract record ReaxNode(SourceLocation Location) : IReaxNode
{
    public abstract IReaxNode[] Children { get; }

    public IReaxValue GetValue(IReaxExecutionContext context) 
    {
        if (this is ExpressionNode expression)
            return expression.Evaluation(context);
        else if (this is LiteralNode number)
            return number;
        else if (this is ObjectNode objectNode)
            return objectNode;
        else if (this is FunctionCallNode functionCall)
            return (GetFunctionResult(functionCall, context).Success
                 ?? GetFunctionResult(functionCall, context).Error) ?? new NullNode(Location);
        else
            throw new InvalidOperationException("Não foi possivel identificar o tipo da variavel!");
    }

    private (IReaxValue? Success, IReaxValue? Error) GetFunctionResult(FunctionCallNode functionCall, IReaxExecutionContext context)
    {
        var function = context.GetFunction(functionCall.Identifier);
        var parameters = functionCall.Parameter.Select(x => x.GetValue(context)).ToArray();
        return function.Invoke(parameters);
    }

}
