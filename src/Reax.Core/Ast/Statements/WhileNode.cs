using Reax.Core.Ast.Expressions;
using Reax.Core.Helpers;
using Reax.Core.Locations;


namespace Reax.Core.Ast.Statements;

public record WhileNode(
    ReaxNode Condition, 
    ContextNode Block, 
    SourceLocation Location) : StatementNode(Location), IBranchFlowNode
{
    public override IReaxNode[] Children => [Condition, Block];

    public override void Execute(IReaxExecutionContext context)
    {
        var condition = (BinaryNode)Condition;

        while((bool)condition.Evaluation(context).Value)
        {
            var interpreter = context.CreateInterpreter(ToString(), Block.Block);
            interpreter.Interpret();
        }
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

        Condition.Serialize(writer);
        Block.Serialize(writer);
        base.Serialize(writer);
    }

    public static new WhileNode Deserialize(BinaryReader reader)
    {
        var condition = BinaryDeserializerHelper.Deserialize<ReaxNode>(reader);
        var block = BinaryDeserializerHelper.Deserialize<ContextNode>(reader);
        var location = ReaxNode.Deserialize(reader);
        return new WhileNode(condition, block, location);
    }

    public override string ToString()
    {
        return $"while {Condition} {{...}}";
    }
}
