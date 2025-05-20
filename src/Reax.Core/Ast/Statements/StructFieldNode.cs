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

    public override string ToString()
    {
        return $"{Identifier}: {Type}";
    }
}
