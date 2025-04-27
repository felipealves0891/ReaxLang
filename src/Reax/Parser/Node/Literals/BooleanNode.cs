using Reax.Parser.Node.Interfaces;
using Reax.Parser.Node.Literals;
using Reax.Runtime;

namespace Reax.Parser.Node.Literals;

public record BooleanNode(
    string Source, 
    SourceLocation Location) : LiteralNode(Source, Location)
{
    public override object Value => bool.Parse(Source.ToLower());

    public override DataType Type => DataType.BOOLEAN;

    public override string ToString()
    {
        return $"{Source.ToLower()}";
    }
}
