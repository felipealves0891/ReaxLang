using System;
using Reax.Core.Ast;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;
using Reax.Parser.Node;
using Reax.Runtime;

namespace Reax.Extensions;

public static class ReaxNodeExtensions
{
    public static IReaxValue GetValue(this ReaxNode node, ReaxExecutionContext context) 
    {
        if(node is NumberNode number)
            return number;
        else if(node is StringNode text)
            return text;
        else if(node is VarNode variable)
            return context.GetVariable(variable.Identifier);
        else if(node is BooleanNode boolean)
            return boolean;
        else if(node is FunctionCallNode functionCall)
            return (GetFunctionResult(functionCall, context).Success 
                 ?? GetFunctionResult(functionCall, context).Error) ?? new NullNode(node.Location);
        else
            throw new InvalidOperationException("Não foi possivel identificar o tipo da variavel!");
    }

    private static (IReaxValue? Success, IReaxValue? Error) GetFunctionResult(FunctionCallNode functionCall, ReaxExecutionContext context)
    {
        var function = context.GetFunction(functionCall.Identifier);
        var parameters = functionCall.Parameter.Select(x => x.GetValue(context)).ToArray();
        return function.Invoke(parameters);
    }

}
