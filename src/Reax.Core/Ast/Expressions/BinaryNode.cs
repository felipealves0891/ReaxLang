using System.Diagnostics.CodeAnalysis;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;
using Reax.Core.Helpers;
using Reax.Core.Locations;

namespace Reax.Core.Ast.Expressions;

[ExcludeFromCodeCoverage]
public record BinaryNode(
    ReaxNode Left,
    ReaxNode Operator,
    ReaxNode Right,
    SourceLocation Location) : ExpressionNode(Location)
{
    public override IReaxNode[] Children => [Left, Operator, Right];

    public override IReaxValue Evaluation(IReaxExecutionContext context)
    {
        var left = Left is BinaryNode
                 ? new BooleanNode(((BinaryNode)Left).Evaluation(context).ToString() ?? "false", Location)
                 : Left.GetValue(context);

        var right = Right is BinaryNode
                 ? new BooleanNode(((BinaryNode)Right).ToString(), Location)
                 : Right.GetValue(context);

        var logical = (ILogicOperator)Operator;
        return new BooleanNode(logical.Compare((ReaxNode)left, (ReaxNode)right).ToString(), Operator.Location);
    }

    public override string ToString()
    {
        return $"{Left} {Operator} {Right}";
    }

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);
        Left.Serialize(writer);
        Operator.Serialize(writer);
        Right.Serialize(writer);
        base.Serialize(writer);
    }

    public static new BinaryNode Deserialize(BinaryReader reader)
    {
        var left = BinaryDeserializerHelper.Deserialize<ReaxNode>(reader);
        var op = BinaryDeserializerHelper.Deserialize<ReaxNode>(reader);
        var right = BinaryDeserializerHelper.Deserialize<ReaxNode>(reader);
        var location = ReaxNode.Deserialize(reader);
        return new BinaryNode(left, op, right, location);
    }
}
