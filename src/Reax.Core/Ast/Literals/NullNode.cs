using System;
using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;
using Reax.Core.Types;
using Reax.Core.Ast.Literals;
using Reax.Core.Ast;

namespace Reax.Parser.Node;

[ExcludeFromCodeCoverage]
public record NullNode(SourceLocation Location) : LiteralNode("NULL", Location)
{
    public override object Value => new object{};
    public override DataType Type => DataType.NULL;
    public override IReaxNode[] Children => [];

    public override string ToString()
    {
        return "NULL";
    }
}
