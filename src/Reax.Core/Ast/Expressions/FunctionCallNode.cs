using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;
using Reax.Core.Helpers;

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

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);

        writer.Write(Identifier);
        writer.Write(Parameter.Length);
        foreach (var param in Parameter)
        {
            param.Serialize(writer);
        }
        base.Serialize(writer);
    }
    
    public static new FunctionCallNode Deserialize(BinaryReader reader)
    {
        var identifier = reader.ReadString();
        var parameterCount = reader.ReadInt32();
        var parameters = new ReaxNode[parameterCount];

        for (int i = 0; i < parameterCount; i++)
            parameters[i] = BinaryDeserializerHelper.Deserialize<ReaxNode>(reader);

        var location = ReaxNode.Deserialize(reader);
        return new FunctionCallNode(identifier, parameters, location);
    }

    public override string ToString()
    {
        return $"{Identifier}({string.Join(',', Parameter.Select(x => x.ToString()))});";
    }
}
