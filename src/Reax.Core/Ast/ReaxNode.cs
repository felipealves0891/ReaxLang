using Reax.Core.Locations;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;
using Reax.Parser.Node;

namespace Reax.Core.Ast;

public abstract record ReaxNode(SourceLocation Location) : IReaxNode
{
    public abstract IReaxNode[] Children { get; }

    public LiteralNode GetValue(IReaxExecutionContext context) 
    {
        if(this is LiteralNode number)
            return number;
        else if(this is VarNode variable)
            return context.GetVariable(variable.Identifier);
        else if(this is FunctionCallNode functionCall)
            return (GetFunctionResult(functionCall, context).Success 
                 ?? GetFunctionResult(functionCall, context).Error) ?? new NullNode(Location);
        else
            throw new InvalidOperationException("Não foi possivel identificar o tipo da variavel!");
    }

    private (LiteralNode? Success, LiteralNode? Error) GetFunctionResult(FunctionCallNode functionCall, IReaxExecutionContext context)
    {
        var function = context.GetFunction(functionCall.Identifier);
        var parameters = functionCall.Parameter.Select(x => x.GetValue(context)).ToArray();
        return function.Invoke(parameters);
    }

}
