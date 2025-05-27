using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;
using Reax.Parser.Node;
using Reax.Core.Helpers;

namespace Reax.Core.Ast.Expressions;

[ExcludeFromCodeCoverage]
public record ExternalFunctionCallNode(
    string scriptName, 
    FunctionCallNode functionCall, 
    SourceLocation Location) : ExpressionNode(Location)
{
    public override IReaxNode[] Children => [];

    public override IReaxValue Evaluation(IReaxExecutionContext context)
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

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);

        writer.Write(scriptName);
        functionCall.Serialize(writer);
        base.Serialize(writer);
    }

    public static new ExternalFunctionCallNode Deserialize(BinaryReader reader)
    {
        var scriptName = reader.ReadString();
        var functionCall = BinaryDeserializerHelper.Deserialize<FunctionCallNode>(reader);
        var location = ReaxNode.Deserialize(reader);

        return new ExternalFunctionCallNode(scriptName, functionCall, location);
    }

    public override string ToString()
    {
        var parameters = functionCall.Parameter.Select(x => x.ToString());
        return $"{scriptName}.{functionCall.Identifier}({string.Join(',', parameters)})";
    }
}
