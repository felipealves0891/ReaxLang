using System;
using Reax.Parser;

namespace Reax.Runtime.Contexts;

public struct Symbol
{
    public Symbol(
        string identifier, 
        DataType type, 
        SymbolCategory category, 
        string scope, 
        SourceLocation sourceLocation)
    {
        Identifier = identifier;
        Type = type;
        Category = category;
        Scope = scope;
        DeclareLocation = sourceLocation;
        Parameters = new();
    }

    public Symbol(
        string identifier, 
        DataType type, 
        SymbolCategory category, 
        string scope, 
        SourceLocation sourceLocation, 
        IEnumerable<Symbol> parameters)
    {
        Identifier = identifier;
        Type = type;
        Category = category;
        Scope = scope;
        DeclareLocation = sourceLocation;
        Parameters = new(parameters);
    }

    public string Identifier { get; private set; }

    public DataType Type { get; private set; }

    public SymbolCategory Category { get; private set; }

    public string Scope { get; private set; }

    public SourceLocation DeclareLocation { get; private set; }

    public bool? Assigned { get; private set; }

    public SourceLocation? AssignedLocation { get; private set; }

    public List<Symbol> Parameters { get; private set; }

    public void MarkAsAssigned(SourceLocation assignedLocation)
    {
        Assigned = true;
        AssignedLocation = assignedLocation;
    }
        

}
