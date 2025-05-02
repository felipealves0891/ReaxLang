using System;
using System.Collections.Concurrent;

namespace Reax.Semantic.Contexts;

public class SemanticContext : ISemanticContext
{
    private readonly ConcurrentDictionary<string, Symbol> _symbolsTable;

    public SemanticContext()
    {
        _symbolsTable = new ConcurrentDictionary<string, Symbol>();
    }

    public ValidationResult Declare(Symbol symbol)
    {
        if(_symbolsTable.ContainsKey(symbol.Identifier))
            return ValidationResult.SymbolAlreadyDeclared(symbol.Identifier);

        _symbolsTable[symbol.Identifier] = symbol;
        return ValidationResult.Success();
    }

    public Symbol? Resolve(string identifier)
    {
        return _symbolsTable.TryGetValue(identifier, out var symbol) ? symbol : null;
    }
}
