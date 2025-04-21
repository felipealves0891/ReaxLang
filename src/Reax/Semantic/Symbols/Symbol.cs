using System;

namespace Reax.Semantic.Symbols;

public struct Symbol
{
    public Symbol(
        string identifier, 
        SymbolType type, 
        SymbolCategoty categoty, 
        Guid scopeId,
        bool? observable = null, 
        bool? immutable = null, 
        bool? async = null) : this()
    {
        Id = Guid.NewGuid();
        Identifier = identifier;
        Type = type;
        Categoty = categoty;
        Observable = observable;
        Immutable = immutable;
        Async = async;
        ScopeId = scopeId;
    }

    public Guid Id { get; private set; }

    public string Identifier { get; private set; }

    public SymbolType Type { get; private set; }

    public SymbolCategoty Categoty { get; private set; }

    public bool? Observable { get; private set; }

    public bool? Immutable { get; private set; }

    public bool? Async { get; private set; }

    public Guid ScopeId { get; private set; }
}
