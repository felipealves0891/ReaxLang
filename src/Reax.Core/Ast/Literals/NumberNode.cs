using System.Diagnostics.CodeAnalysis;
using System.Text;
using Reax.Core.Locations;
using Reax.Core.Types;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;
using System.Numerics;

namespace Reax.Core.Ast.Literals;

[ExcludeFromCodeCoverage]
public record NumberNode(
    string Source, 
    SourceLocation Location) : LiteralNode(Source, Location)
{
    public override object Value
    {
        get
        {
            if (Source.IndexOf('.') == -1 && Source.IndexOf(',') == -1)
                return int.Parse(Source);
            else
                return decimal.Parse(Source);
        }
    }

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);

        writer.Write(Source);
        base.Serialize(writer);
    }

    public static new NumberNode Deserialize(BinaryReader reader)
    {
        var source = reader.ReadString();
        var location = ReaxNode.Deserialize(reader);
        return new NumberNode(source, location);
    }

    public override DataType Type => DataType.NUMBER;
    public override IReaxNode[] Children => [];

    public override string ToString()
    {
        return $"{Source}";
    }
}
