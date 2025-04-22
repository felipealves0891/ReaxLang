using System;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Scopes;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record NullNode(SourceLocation Location) : ReaxNode(Location), IReaxType
{
    public SymbolType GetReaxType(IReaxScope scope)
    {
        return SymbolType.NONE;
    }
}
