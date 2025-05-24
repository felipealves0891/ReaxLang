using System;
using Reax.Core.Locations;
using Reax.Core.Types;

namespace Reax.Core.Ast.Statements;

public record StructFieldNode(
    string Identifier,
    DataType Type,
    SourceLocation Location) : ReaxNode(Location)
{
    public override IReaxNode[] Children => [];

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);

        writer.Write(Identifier);
        writer.Write((int)Type);
        base.Serialize(writer);
    }

    public override string ToString()
    {
        return $"{Identifier}: {Type}";
    }
}
