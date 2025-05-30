using System;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Helpers;
using Reax.Core.Locations;
using Reax.Core.Types;

namespace Reax.Core.Ast.Objects;

public record StructInstanceNode(
    string Name,
    Dictionary<string, ReaxNode> FieldValues,
    SourceLocation Location) : ObjectNode(Location)
{
    public override IReaxNode[] Children => FieldValues.Values.Cast<IReaxNode>().ToArray();
    public override object Value => FieldValues;
    public override DataType Type => DataType.STRUCT;

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);

        writer.Write(Name);
        writer.Write(FieldValues.Count);
        foreach (var field in FieldValues)
        {
            writer.Write(field.Key);
            field.Value.Serialize(writer);
        }
        base.Serialize(writer);
    }

    public static new StructInstanceNode Deserialize(BinaryReader reader)
    {
        var name = reader.ReadString();
        var fieldCount = reader.ReadInt32();
        var fieldValues = new Dictionary<string, ReaxNode>(fieldCount);
        for (var i = 0; i < fieldCount; i++)
        {
            var key = reader.ReadString();
            var value = BinaryDeserializerHelper.Deserialize<ReaxNode>(reader);
            fieldValues[key] = value;
        }
        var location = ReaxNode.Deserialize(reader);
        return new StructInstanceNode(name, fieldValues, location);
    }
    
    public override string ToString()
    {
        return $"{Name} {{}}";
    }
}
