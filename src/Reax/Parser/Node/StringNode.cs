using Reax.Parser.Node.Interfaces;
using Reax.Runtime;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Scopes;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record StringNode(string Value, 
    SourceLocation Location) : ReaxNode(Location), IReaxValue, IReaxType
{
    public SymbolType GetReaxType(IReaxScope scope)
    {
        return SymbolType.STRING;
    }

    public override string ToString()
    {
        return $"'{Value}'";
    }
}
