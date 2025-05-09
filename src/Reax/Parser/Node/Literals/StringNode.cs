using Reax.Parser.Node.Interfaces;
using Reax.Parser.Node.Literals;
using Reax.Runtime;
using Reax.Semantic;

namespace Reax.Parser.Node.Literals;

public record StringNode(
    string Source, 
    SourceLocation Location) : LiteralNode(Source, Location)
{
    public override object Value => Source;
    public override DataType Type => DataType.STRING;
    public override IReaxNode[] Children => [];

    public override string ToString()
    {
        return $"'{Source}'";
    }
}
