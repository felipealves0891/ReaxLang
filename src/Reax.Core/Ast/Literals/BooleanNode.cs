using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;
using Reax.Core.Types;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;

namespace Reax.Core.Ast.Literals;

[ExcludeFromCodeCoverage]
public record BooleanNode(
    string Source, 
    SourceLocation Location) : LiteralNode(Source, Location)
{
    public override object Value => bool.Parse(Source.ToLower());
    public override DataType Type => DataType.BOOLEAN;
    public override IReaxNode[] Children => [];

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);

        writer.Write(Source);
        base.Serialize(writer);
    }

    public static new BooleanNode Deserialize(BinaryReader reader)
    {
        var source = reader.ReadString();
        var location = ReaxNode.Deserialize(reader);
        return new BooleanNode(source, location);
    }

    public override string ToString()
    {
        return $"{Source.ToLower()}";
    }
}
