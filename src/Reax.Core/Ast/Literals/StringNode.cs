using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;
using Reax.Core.Types;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;

namespace Reax.Core.Ast.Literals;

[ExcludeFromCodeCoverage]
public record StringNode(
    string Source, 
    SourceLocation Location) : LiteralNode(Source, Location)
{
    public override object Value => Source;
    public override DataType Type => DataType.STRING;
    public override IReaxNode[] Children => [];

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);

        writer.Write(Source);
        base.Serialize(writer);
    }

    public override string ToString()
    {
        return $"'{Source}'";
    }
}
