using Reax.Core.Ast.Expressions;
using Reax.Core.Helpers;
using Reax.Core.Locations;
using Reax.Core.Types;


namespace Reax.Core.Ast.Statements;

public record FunctionDeclarationNode(
    string Identifier, 
    ContextNode Block, 
    VarNode[] Parameters, 
    DataType SuccessType,
    DataType ErrorType,
    SourceLocation Location) : StatementNode(Location), IControlFlowNode, IReaxDeclaration
{
    public override IReaxNode[] Children => Parameters.Cast<ReaxNode>().Concat(Block.Block.Cast<ReaxNode>()).ToArray();
    
    public DataType ResultSuccess => SuccessType;

    public DataType ResultError => ErrorType;

    public override void Execute(IReaxExecutionContext context)
    {
        var interpreter = context.CreateInterpreter(ToString(), Block.Block, Parameters);
        context.Declare(Identifier);
        context.SetFunction(Identifier, interpreter);
    }

    public bool HasGuaranteedReturn()
    {
        return Block.HasGuaranteedReturn();
    }

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);

        writer.Write(Identifier);
        Block.Serialize(writer);
        writer.Write(Parameters.Length);
        foreach (var param in Parameters)
        {
            param.Serialize(writer);
        }
        writer.Write((int)SuccessType);
        writer.Write((int)ErrorType);
        base.Serialize(writer);
    }

    public static new FunctionDeclarationNode Deserialize(BinaryReader reader)
    {
        var identifier = reader.ReadString();
        var block = BinaryDeserializerHelper.Deserialize<ContextNode>(reader);
        var paramCount = reader.ReadInt32();
        var parameters = new VarNode[paramCount];
        for (var i = 0; i < paramCount; i++)
        {
            parameters[i] = BinaryDeserializerHelper.Deserialize<VarNode>(reader);
        }
        var successType = (DataType)reader.ReadInt32();
        var errorType = (DataType)reader.ReadInt32();
        var location = ReaxNode.Deserialize(reader);
        return new FunctionDeclarationNode(identifier, block, parameters, successType, errorType, location);
    }

    public override string ToString()
    {
        var param = string.Join(',', Parameters.Select(x => x.ToString()));
        return $"fun {Identifier} ({param}):{SuccessType} | {ErrorType} {{...}}";
    }
}
