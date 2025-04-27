using System;
using Reax.Lexer;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Scopes;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record MatchNode(
    ReaxNode Expression,    
    ActionNode Success,
    ActionNode Error,  
    SourceLocation Location) : ReaxNode(Location), IReaxType, IReaxContext
{
    public ReaxNode[] Context => [Success, Error];

    public Symbol[] GetParameters(Guid scope)
        => [];

    public SymbolType? GetReaxErrorType(IReaxScope scope)
    {
        return ((IReaxType)Error).GetReaxErrorType(scope);
    }

    public SymbolType GetReaxType(IReaxScope scope)
    {
        return ((IReaxType)Success).GetReaxType(scope);
    }

    public override string ToString()
    {
        return $"match {Expression} {{ success, error }}";
    }
}
