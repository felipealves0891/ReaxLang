using System;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record ScriptDeclarationNode(string Identifier, SourceLocation Location) 
    : ReaxNode(Location), IReaxDeclaration
{
    public Symbol GetSymbol(Guid scope, string? module = null)
    {
        return new Symbol(
            Identifier,
            SymbolType.NONE,
            SymbolCategoty.SCRIPT,
            scope,
            parentName: module
        );
    }

    public override string ToString()
    {
        return $"script {Identifier};";
    }
}
