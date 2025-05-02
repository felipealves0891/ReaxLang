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
    SCRIPT
}

public class Symbol
{
    public Symbol(
        string identifier, 
        DataType type, 
        SymbolCategory category)
    {
        Identifier = identifier;
        Type = type;
        Category = category;
    }

    public string Identifier { get; private set; }

    public DataType Type { get; private set; }

    public SymbolCategory Category { get; private set; }

    public static Symbol CreateFunction(string identifier, DataType type)
        => new Symbol(identifier, type, SymbolCategory.FUNCTION);
    
    public static Symbol CreateConst(string identifier, DataType type)
        => new Symbol(identifier, type, SymbolCategory.CONST);
        
    public static Symbol CreateLet(string identifier, DataType type)
        => new Symbol(identifier, type, SymbolCategory.LET_SYNC);
    public static Symbol CreateLetAsync(string identifier, DataType type)
        => new Symbol(identifier, type, SymbolCategory.LET_ASYNC);
        
    public static Symbol CreateBind(string identifier, DataType type)
        => new Symbol(identifier, type, SymbolCategory.BIND);
        
    public static Symbol CreateModule(string identifier)
        => new Symbol(identifier, DataType.NONE, SymbolCategory.MODULE);
        
    public static Symbol CreateScript(string identifier)
        => new Symbol(identifier, DataType.NONE, SymbolCategory.SCRIPT);

}
