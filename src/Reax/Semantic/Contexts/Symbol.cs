using System;
using Reax.Parser;

namespace Reax.Semantic.Contexts;

public enum SymbolCategory 
{
    NONE,
    LET_SYNC,
    LET_ASYNC,
    CONST,
    BIND,
    FUNCTION,
    MODULE,
    SCRIPT,
    PARAMETER,
    PARAMETER_OPTIONAL
}

public record Symbol
{
    public Symbol(
        string identifier,
        DataType type,
        SymbolCategory category,
        SourceLocation location,
        string? parentIdentifier = null)
    {
        Identifier = identifier;
        Type = type;
        Category = category;
        Location = location;
        ParentIdentifier = parentIdentifier;
    }

    public string Identifier { get; private set; }

    public DataType Type { get; private set; }

    public SymbolCategory Category { get; private set; }

    public string? ParentIdentifier { get; set; }

    public SourceLocation Location { get; private set; }

    public static Symbol CreateFunction(string identifier, DataType type, SourceLocation location)
        => new Symbol(identifier, type, SymbolCategory.FUNCTION, location);
    
    public static Symbol CreateConst(string identifier, DataType type, SourceLocation location)
        => new Symbol(identifier, type, SymbolCategory.CONST, location);
        
    public static Symbol CreateLet(string identifier, DataType type, SourceLocation location)
        => new Symbol(identifier, type, SymbolCategory.LET_SYNC, location);
    public static Symbol CreateLetAsync(string identifier, DataType type, SourceLocation location)
        => new Symbol(identifier, type, SymbolCategory.LET_ASYNC, location);
        
    public static Symbol CreateBind(string identifier, DataType type, SourceLocation location)
        => new Symbol(identifier, type, SymbolCategory.BIND, location);
        
    public static Symbol CreateModule(string identifier, SourceLocation location)
        => new Symbol(identifier, DataType.NONE, SymbolCategory.MODULE, location);
        
    public static Symbol CreateScript(string identifier, SourceLocation location)
        => new Symbol(identifier, DataType.NONE, SymbolCategory.SCRIPT, location);
        
    public static Symbol CreateParameter(string identifier, string parentIdentifier, DataType type, SourceLocation location)
        => new Symbol(identifier, type, SymbolCategory.PARAMETER, location, parentIdentifier);
        
    public static Symbol CreateParameterOptional(string identifier, string parentIdentifier, DataType type, SourceLocation location)
        => new Symbol(identifier, type, SymbolCategory.PARAMETER_OPTIONAL, location, parentIdentifier);

}
