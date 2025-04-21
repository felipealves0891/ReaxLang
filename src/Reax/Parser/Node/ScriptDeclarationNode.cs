using System;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record ScriptDeclarationNode(string Identifier, SourceLocation Location) 
    : ReaxNode(Location), IReaxDeclaration
{
    public Symbol GetSymbol(Guid scope)
    {
        return new Symbol(
            Identifier,
            SymbolType.NONE,
            SymbolCategoty.SCRIPT,
            scope
        );
    }

    public override string ToString()
    {
        return $"script {Identifier};";
    }
}
