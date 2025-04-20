using Reax.Parser.Node.Interfaces;
using Reax.Runtime;

namespace Reax.Parser.Node;

public record StringNode(string Value, 
    SourceLocation Location) : ReaxNode(Location), IReaxValue
{
    public SymbolType GetDataType()
    {
        return SymbolType.STRING;
    }

    public override string ToString()
    {
        return $"'{Value}'";
    }
}
