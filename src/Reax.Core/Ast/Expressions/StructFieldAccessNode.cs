using System;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Locations;

namespace Reax.Core.Ast.Objects.Structs;

public record StructFieldAccessNode(
    string Identifier,
    string Property,
    SourceLocation Location)
    : ExpressionNode(Location)
{
    public override IReaxNode[] Children => [];

    public override IReaxValue Evaluation(IReaxExecutionContext context)
    {
        var variable = context.GetVariable(Identifier);
        var obj = (StructInstanceNode)variable;
        return obj.FieldValues[Property].GetValue(context);
    }

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);
        writer.Write(Identifier);
        writer.Write(Property);
        base.Serialize(writer);
    }

    public static new StructFieldAccessNode Deserialize(BinaryReader reader)
    {
        var identifier = reader.ReadString();
        var property = reader.ReadString();
        var location = ReaxNode.Deserialize(reader);
        return new StructFieldAccessNode(identifier, property, location);
    }

    public override string ToString()
    {
        return $"{Identifier}->{Property}";
    }
}
