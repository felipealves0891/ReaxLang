using Reax.Parser.Node.Interfaces;
using Reax.Parser.Node.Literals;
using Reax.Runtime;

namespace Reax.Parser.Node.Literals;

public record StringNode(
    string Source, 
    SourceLocation Location) : LiteralNode(Source, Location)
{
    public override object Value => Source;

    public override string ToString()
    {
        return $"'{Source}'";
    }
}
