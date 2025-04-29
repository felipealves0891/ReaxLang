using System;
using Reax.Parser;

namespace Reax.Semantic.Symbols;

public class Symbol
{
    public Symbol(string identifier, DataType successType, DataType errorType, SymbolCategory category, SourceLocation location, bool? immutable = null, bool? async = null, string? parentIdentifier = null)
    {
        Identifier = identifier;
        SuccessType = successType;
        ErrorType = errorType;
        Category = category;
        Immutable = immutable;
        Async = async;
        ParentIdentifier = parentIdentifier;
        Location = location;
        Assigned = false;
    }

    public string Identifier { get; private set; }

    public DataType SuccessType { get; private set; }

    public DataType ErrorType { get; private set; }

    public SymbolCategory Category { get; private set; }

    public bool? Immutable { get; private set; }

    public bool? Async { get; private set; }

    public bool? Assigned { get; private set; }

    public string? ParentIdentifier { get; private set; }

    public SourceLocation Location { get; private set; }

    public void MarkAsAssigned()
        => Assigned = true;
}
