using System;
using Reax.Parser.Node.Literals;
using Reax.Semantic;

namespace Reax.Parser.Node;

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
