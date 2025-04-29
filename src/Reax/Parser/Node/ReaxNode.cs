using Reax.Parser.Node.Interfaces;
using Reax.Parser.Node.Literals;
using Reax.Runtime;

namespace Reax.Parser.Node;

public abstract record ReaxNode(SourceLocation Location)
{
    public ReaxNode GetValue(ReaxExecutionContext context) 
    {
        if(this is NumberNode number)
            return number;
        else if(this is StringNode text)
            return text;
        else if(this is VarNode variable)
            return context.GetVariable(variable.Identifier);
        else if(this is BooleanNode boolean)
            return boolean;
        else if(this is FunctionCallNode functionCall)
            return (GetFunctionResult(functionCall, context).Success 
                 ?? GetFunctionResult(functionCall, context).Error) ?? new NullNode(Location);
        else
            throw new InvalidOperationException("Não foi possivel identificar o tipo da variavel!");
    }

    private (ReaxNode? Success, ReaxNode? Error) GetFunctionResult(FunctionCallNode functionCall, ReaxExecutionContext context)
    {
        var function = context.GetFunction(functionCall.Identifier);
            var parameters = functionCall.Parameter.Select(x => x.GetValue(context)).ToArray();
            return function.Invoke(parameters);
    }
}
