using System;
using Reax.Runtime.Symbols;

namespace Reax.Runtime;

public struct Symbol
{
    public Symbol()
        => Id = Guid.NewGuid();

    public Symbol(SymbolCategoty categoty) 
        : this()
    {
        Categoty = categoty;
    }

    public Guid Id { get; set; }

    public SymbolType Type { get; set; }

    public SymbolCategoty Categoty { get; set; }

    public bool? Observable { get; set; }

    public bool? Immutable { get; set; }

    public bool? Async { get; set; }
}
