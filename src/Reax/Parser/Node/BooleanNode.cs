using Reax.Parser.Node.Interfaces;
using Reax.Runtime;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Scopes;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record BooleanNode(string Value, 
    SourceLocation Location) : ReaxNode(Location), IReaxValue, IReaxType
{
    public bool ValueConverted => bool.Parse(Value);

    public SymbolType GetReaxType(IReaxScope scope)
    {
        return SymbolType.BOOLEAN;
    }

    public override string ToString()
    {
        return $"{Value}";
    }
}
