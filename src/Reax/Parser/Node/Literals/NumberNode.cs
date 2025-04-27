using System.Text;
using Reax.Parser.Node.Interfaces;
using Reax.Parser.Node.Literals;
using Reax.Runtime;

namespace Reax.Parser.Node.Literals;

public record NumberNode(
    string Source, 
    SourceLocation Location) : LiteralNode(Source, Location)
{
    public override object Value => decimal.Parse(Source);

    public override DataType Type => DataType.NUMBER;

    public override string ToString()
    {
        return $"{Source}";
    }
}
