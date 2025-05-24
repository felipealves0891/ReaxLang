using System;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;
using Reax.Core.Ast.Objects;
using Reax.Core.Locations;
using Reax.Core.Types;

namespace Reax.Core.Ast.Expressions;

public record NativeCallNode(
    ReaxNode Node,
    DataType Type,
    SourceLocation Location) : ExpressionNode(Location)
{
    public override IReaxNode[] Children => [Node];

    public override IReaxValue Evaluation(IReaxExecutionContext context)
    {
        if (Node is LiteralNode literal)
            return literal;

        var result = ((ExpressionNode)Node).Evaluation(context);
        if (result is not NativeValueNode)
            return result;

        return TypeResolverHelper.CastToReax(
            result.Value,
            result.Value.ToString() ?? "",
            Type,
            Location);
    }

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);

        Node.Serialize(writer);
        writer.Write(((int)Type));
        base.Serialize(writer);
    }

    public override string ToString()
    {
        return $"{Node} as {Type}";
    }
}
