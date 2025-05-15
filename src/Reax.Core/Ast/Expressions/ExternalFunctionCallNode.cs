using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;

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
            interpreter.ExecuteFunctionCall(new FunctionCallNode(identifier, parameters, Location));
            return interpreter.Output ?? interpreter.Error ?? throw new Exception();
        }
        else if(context.ModuleExists(scriptName)) 
        {
            var function = context.GetModule(scriptName, functionCall.Identifier);
            var parameters = functionCall.Parameter.Select(x => x.GetValue(context)).ToArray();
            var identifier = functionCall.Identifier;
            var (success, error) = function.Invoke(parameters);
            return success ?? error ?? throw new Exception();
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
