using System;
using Reax.Runtime.Symbols;

namespace Reax.Runtime.Registries;

public class SymbolRegistry : BaseRegistry<string, Symbol>
{
    public void UpdateSymbol(string key, SymbolCategoty categoty) 
    {
        var symbol = this[key];
        symbol.Categoty = categoty;
        this[key] = symbol;
    }

    public void UpdateSymbol(string key, string dataType, bool immutable, bool isAsync, SymbolCategoty categoty) 
    {
        var symbol = this[key];
        symbol.Immutable = immutable;
        symbol.Async = isAsync;
        symbol.Observable = categoty == SymbolCategoty.LET && !immutable;
        symbol.Categoty = categoty;

        symbol.Type = Enum.TryParse<SymbolType>(dataType, true, out var type) 
                    ? type 
                    : throw new InvalidDataException($"Tipo da dado invalido: {dataType}!");

        this[key] = symbol;
    }

    protected override Symbol Load(string key)
    {
        throw new NotImplementedException();
    }
}
