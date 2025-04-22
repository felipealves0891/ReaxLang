using Reax.Parser.Node.Interfaces;
using Reax.Runtime;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Scopes;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record NumberNode(
    string Value, 
    SourceLocation Location) : ReaxNode(Location), IReaxValue, IReaxType
{
    public decimal ValueConverted => decimal.Parse(Value);

    public SymbolType GetReaxType(IReaxScope scope)
    {
        if(int.TryParse(Value, out var _))
            return SymbolType.INT;
        else if(long.TryParse(Value, out var _))
            return SymbolType.LONG;
        else if(decimal.TryParse(Value, out var _))
            return SymbolType.FLOAT;
        else    
            return SymbolType.NONE;
    }

    public override string ToString()
    {
        return $"{Value}";
    }
}
