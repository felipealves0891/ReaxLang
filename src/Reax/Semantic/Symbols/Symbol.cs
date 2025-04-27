using System;
using Reax.Parser;

namespace Reax.Semantic.Symbols;

public class Symbol
{
    public Symbol(
        string identifier, 
        DataType type, 
        SymbolCategory category,
        SourceLocation location,
        string? context = null, 
        bool? immutable = null, 
        bool? async = null)
    {
        Identifier = identifier;
        Type = type;
        Immutable = immutable;
        Async = async;
        Category = category;
        Context = context;
        Location = location;        
    }

    public string Identifier { get; private set; }

    public DataType Type { get; private set; }

    public bool? Immutable { get; private set; }

    public bool? Async { get; private set; }

    public bool? Assigned { get; private set; }

    public SymbolCategory Category { get; private set; }

    public string? Context { get; private set; }

    public SourceLocation Location { get; private set; }
    
    public void MarkAsAssigned()
        => Assigned = true;
}


