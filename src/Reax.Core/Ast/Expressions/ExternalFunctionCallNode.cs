using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;
using Reax.Parser.Node;

namespace Reax.Core.Ast.Expressions;

[ExcludeFromCodeCoverage]
public record ExternalFunctionCallNode(
    string scriptName, 
    FunctionCallNode functionCall, 
    SourceLocation Location) : ExpressionNode(Location)
{
    public override IReaxNode[] Children => [];

    public override LiteralNode Evaluation(IReaxExecutionContext context)
    {
        if(context.ScriptExists(scriptName))
        {
            var interpreter = context.GetScript(scriptName);
            var parameters = functionCall.Parameter.Select(x => x.GetValue(context)).ToArray();
            var identifier = functionCall.Identifier;
            interpreter.ExecuteFunctionCall(functionCall);
            if (interpreter.Error is not null)
                throw new ReturnErrorException(interpreter.Error);

            return interpreter.Output ?? new NullNode(Location);
        }
        else if(context.ModuleExists(scriptName)) 
        {
            var function = context.GetModule(scriptName, functionCall.Identifier);
            var parameters = functionCall.Parameter.Select(x => x.GetValue(context)).ToArray();
            var identifier = functionCall.Identifier;
            var (success, error) = function.Invoke(parameters);
            if (error is not null)
                throw new ReturnErrorException(error);

            return success ?? new NullNode(Location);
        }
        else 
        {
            throw new InvalidOperationException($"Função externa não localizada: {scriptName}.{functionCall.Identifier}"); 
        }
    }

    public override string ToString()
    {
        var parameters = functionCall.Parameter.Select(x => x.ToString());
        return $"{scriptName}.{functionCall.Identifier}({string.Join(',', parameters)})";
    }
}
