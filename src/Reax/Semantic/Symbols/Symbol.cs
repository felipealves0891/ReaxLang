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
        bool? async = null,
        string? parentName = null,
        SymbolType? errorType = null) : this()
    {
        Id = Guid.NewGuid();
        Identifier = identifier;
        SuccessType = type;
        Categoty = categoty;
        Observable = observable;
        Immutable = immutable;
        Async = async;
        ScopeId = scopeId;
        ParentName = parentName;
        ErrorType = errorType;
    }

    public Symbol(
        string identifier, 
        string type, 
        SymbolCategoty categoty, 
        Guid scopeId,
        bool? observable = null, 
        bool? immutable = null, 
        bool? async = null,
        string? parentName = null,
        string? errorType = null) : this()
    {
        Id = Guid.NewGuid();
        Identifier = identifier;
        Categoty = categoty;
        Observable = observable;
        Immutable = immutable;
        Async = async;
        ScopeId = scopeId;
        ParentName = parentName;

        SuccessType = Enum.TryParse<SymbolType>(type, true, out var result) 
             ? result 
             : throw new InvalidDataException($"Tipo {type} não localizado!");

        if(errorType is not null )
        {
            ErrorType = Enum.TryParse<SymbolType>(errorType, true, out var resultError) 
                ? resultError
                : throw new InvalidDataException($"Tipo {errorType} não localizado!");
        }
    }

    public Guid Id { get; private set; }

    public string Identifier { get; private set; }

    public SymbolType SuccessType { get; private set; }

    public SymbolType? ErrorType { get; private set; }

    public SymbolCategoty Categoty { get; private set; }

    public bool? Observable { get; private set; }

    public bool? Immutable { get; private set; }

    public bool? Async { get; private set; }

    public bool? Assigned { get; set; }

    public string? ParentName { get; set; }

    public Guid ScopeId { get; private set; }
}
