using Reax.Parser.Node.Interfaces;
using Reax.Runtime;

namespace Reax.Parser.Node;

public record NumberNode(
    string Value, 
    SourceLocation Location) : ReaxNode(Location), IReaxValue
{
    public decimal ValueConverted => decimal.Parse(Value);

    public SymbolType GetDataType()
    {
        if(int.TryParse(Value, out var _))
            return SymbolType.INT;
        else if(long.TryParse(Value, out var _))
            return SymbolType.LONG;
        else if(float.TryParse(Value, out var _))
            return SymbolType.FLOAT;
        else
            return SymbolType.NONE;
    }

    public override string ToString()
    {
        return $"{Value}";
    }
}
