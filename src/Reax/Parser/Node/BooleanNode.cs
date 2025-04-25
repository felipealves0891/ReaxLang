using Reax.Parser.Node.Interfaces;
using Reax.Runtime;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Scopes;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record BooleanNode(string Value, 
    SourceLocation Location) : ReaxNode(Location), IReaxValue, IReaxType
{
    public object ValueConverted => bool.Parse(Value.ToLower());

    public SymbolType GetReaxType(IReaxScope scope)
    {
        return SymbolType.BOOL;
    }

    public override string ToString()
    {
        return $"{Value.ToLower()}";
    }
}
