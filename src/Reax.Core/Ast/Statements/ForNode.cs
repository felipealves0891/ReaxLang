using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Literals;
using Reax.Core.Helpers;
using Reax.Core.Locations;


namespace Reax.Core.Ast.Statements;

public record ForNode(
    DeclarationNode Declaration, 
    ReaxNode Condition, 
    ContextNode Block, 
    SourceLocation Location) : StatementNode(Location), IBranchFlowNode
{
    public override IReaxNode[] Children => [Declaration, Condition, Block];

    public override void Execute(IReaxExecutionContext context)
    {
        var condition = (BinaryNode)Condition;

        Declaration.Execute(context);
        while((bool)condition.Evaluation(context).Value)
        {
            var interpreter = context.CreateInterpreter(ToString(), Block.Block);
            interpreter.Interpret();

            var value = context.GetVariable(Declaration.Identifier) as NumberNode;
            if(value is null)
                throw new InvalidOperationException("Não foi possivel obter o controlador do loop");

            var intValue = Convert.ToInt32(value.Value) + 1;
            var newValue = new NumberNode(intValue.ToString(), Declaration.Location);
            context.SetVariable(Declaration.Identifier, newValue);
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

        Declaration.Serialize(writer);
        Condition.Serialize(writer);
        Block.Serialize(writer);
        base.Serialize(writer);
    }

    public static new ForNode Deserialize(BinaryReader reader)
    {
        var declaration = BinaryDeserializerHelper.Deserialize<DeclarationNode>(reader);
        var condition = BinaryDeserializerHelper.Deserialize<BinaryNode>(reader);
        var block = BinaryDeserializerHelper.Deserialize<ContextNode>(reader);
        var location = ReaxNode.Deserialize(reader);
        return new ForNode(declaration, condition, block, location);
    }

    public override string ToString()
    {
        return $"for {Declaration} to {Condition} {{...}}";
    }
}
