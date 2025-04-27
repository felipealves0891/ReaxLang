using System;
using System.Collections.Concurrent;
using Reax.Parser.Node.Interfaces;
using Reax.Semantic.Symbols;

namespace Reax.Semantic;

public class SemanticContext : ISemanticContext
{
    private readonly ConcurrentDictionary<string, Symbol> _symbols;

    public SemanticContext()
    {
        _symbols = new ConcurrentDictionary<string, Symbol>();
    }

    public IValidateResult SetSymbol(Symbol symbol)
    {
        if(_symbols.ContainsKey(symbol.Identifier))
            return ValidationResult.ErrorAlreadyDeclared(symbol.Identifier, symbol.Location);

        if(!_symbols.TryAdd(symbol.Identifier, symbol))
            return ValidationResult.ErrorAlreadyDeclared(symbol.Identifier, symbol.Location);

        return ValidationResult.Success(symbol.Location);
    }

    public Symbol? SetSymbol(string identifier)
    {
        return _symbols.TryGetValue(identifier, out var symbol) ? symbol : null;
    }
}
