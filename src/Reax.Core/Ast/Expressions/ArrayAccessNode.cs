using System;
using System.Collections.Immutable;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;
using Reax.Core.Ast.Objects;
using Reax.Core.Ast.Objects.Structs;
using Reax.Core.Helpers;
using Reax.Core.Locations;

namespace Reax.Core.Ast.Expressions;

public record ArrayAccessNode(ReaxNode Array, ReaxNode Expression, SourceLocation Location)
    : ExpressionNode(Location)
{
    public override IReaxNode[] Children => [Array, Expression];

    public override IReaxValue Evaluation(IReaxExecutionContext context)
    {
        var array = GetArray(context);
        var index = GetIndex(context);
        return array[index].GetValue(context);
    }

    private ArrayNode GetArray(IReaxExecutionContext context)
    {
        if (Array is VarNode var)
            return (ArrayNode)context.GetVariable(var.Identifier);
        else if (Array is StructFieldAccessNode fieldAccessNode)
            return (ArrayNode)fieldAccessNode.Evaluation(context);
        else
            throw new InvalidOperationException();
    }

    private int GetIndex(IReaxExecutionContext context)
    {
        if (Expression is ExpressionNode expression)
            return int.Parse(expression.Evaluation(context).Value.ToString() ?? "0");
        else if (Expression is NumberNode literal)
            return int.Parse(literal.Source);
        else
            throw new Exception("Indice invalido!");
    }

    public override string ToString()
    {
        return $"{Array}[{Expression}]";
    }

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);
        Array.Serialize(writer);
        Expression.Serialize(writer);
        base.Serialize(writer);
    }

    public static new ArrayAccessNode Deserialize(BinaryReader reader)
    {
        var array = BinaryDeserializerHelper.Deserialize<ReaxNode>(reader);
        var expression = BinaryDeserializerHelper.Deserialize<ReaxNode>(reader);
        var location = ReaxNode.Deserialize(reader);
        return new ArrayAccessNode(array, expression, location);
    }
}
