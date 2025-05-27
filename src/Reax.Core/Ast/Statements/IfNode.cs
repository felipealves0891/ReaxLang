using Reax.Core.Locations;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Helpers;


namespace Reax.Core.Ast.Statements;

public record IfNode(
    BinaryNode Condition, 
    ContextNode True, 
    ContextNode? False, 
    SourceLocation Location) : StatementNode(Location), IBranchFlowNode
{
    public override IReaxNode[] Children => False is null ? [Condition, True] : [Condition, True, False];

    public override void Execute(IReaxExecutionContext context)
    {
        var left = Condition.Left.GetValue(context);
        var right = Condition.Right.GetValue(context);
        var logical = (ILogicOperator)Condition.Operator;
        var result = logical.Compare((ReaxNode)left, (ReaxNode)right);
        if(result)
        {
            var interpreter = context.CreateInterpreter(ToString(), True.Block);
            interpreter.Interpret(rethrow: true);
        }
        else if(False is not null)
        {
            var interpreter = context.CreateInterpreter(ToString(), False.Block);
            interpreter.Interpret(rethrow: true);
        }
    }

    public bool HasGuaranteedReturn()
    {
        var hasGuaranteedReturn = true;
        if(False is not null)
            hasGuaranteedReturn = False.HasGuaranteedReturn();

        return hasGuaranteedReturn && True.HasGuaranteedReturn();
    }

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);

        Condition.Serialize(writer);
        True.Serialize(writer);
        writer.Write(False is not null ? (byte)1 : (byte)0);
        if (False is not null)
            False.Serialize(writer);
        base.Serialize(writer);
    }

    public static new IfNode Deserialize(BinaryReader reader)
    {
        var condition = BinaryDeserializerHelper.Deserialize<BinaryNode>(reader);
        var trueBlock = BinaryDeserializerHelper.Deserialize<ContextNode>(reader);
        ContextNode? falseBlock = null;
        if (reader.ReadByte() == 1)
            falseBlock = BinaryDeserializerHelper.Deserialize<ContextNode>(reader);

        var location = ReaxNode.Deserialize(reader);
        return new IfNode(condition, trueBlock, falseBlock, location);
    }

    public override string ToString()
    {
        var elseText = False is null ? "" : "else {}";
        return $"if {Condition} {{}} {elseText}";
    }
}
