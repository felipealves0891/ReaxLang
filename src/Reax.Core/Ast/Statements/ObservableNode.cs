using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Helpers;


namespace Reax.Core.Ast.Statements;

[ExcludeFromCodeCoverage]
public record ObservableNode(
    VarNode Var, 
    ContextNode Block, 
    BinaryNode? Condition, 
    SourceLocation Location) : StatementNode(Location)
{
    public override IReaxNode[] Children => Condition is null ? [Var, Block] : [Var, Block, Condition];

    public override void Execute(IReaxExecutionContext context)
    {
        var identifier = Var.Identifier;
        var interpreter = context.CreateInterpreter(ToString(), Block.Block);
        context.SetObservable(identifier, interpreter, Condition);
    }

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);

        Var.Serialize(writer);
        Block.Serialize(writer);
        writer.Write(Condition is not null ? (byte)1 : (byte)0);
        if (Condition is not null)
            Condition.Serialize(writer);
        base.Serialize(writer);
    }

    public static new ObservableNode Deserialize(BinaryReader reader)
    {
        var var = BinaryDeserializerHelper.Deserialize<VarNode>(reader);
        var block = BinaryDeserializerHelper.Deserialize<ContextNode>(reader);
        BinaryNode? condition = null;
        
        if (reader.ReadByte() == 1)
            condition = BinaryDeserializerHelper.Deserialize<BinaryNode>(reader);
        
        var location = ReaxNode.Deserialize(reader);
        return new ObservableNode(var, block, condition, location);
    }

    public override string ToString()
    {
        var when = Condition is null ? "" : $"when {Condition} ";
        return $"on {Var} {when} {{...}}";
    }
}
