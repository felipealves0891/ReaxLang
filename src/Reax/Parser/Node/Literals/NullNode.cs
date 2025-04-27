using System;
using Reax.Parser.Node.Literals;

namespace Reax.Parser.Node;

public record NullNode(SourceLocation Location) : LiteralNode("NULL", Location)
{
    public override object Value => new object{};
    public override DataType Type => DataType.NULL;

    public override string ToString()
    {
        return "NULL";
    }
}
