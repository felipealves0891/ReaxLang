namespace Reax.Semantic.Symbols;

public enum SymbolType
{
    NONE,
    BOOL,
    FLOAT,
    INT,
    LONG,
    STRING,
    VOID
}

public static class SymbolTypeExtensions
{
    public static bool IsCompatible(this SymbolType a, SymbolType b) 
    {
        return (a == b || (a.IsNumber() && b.IsNumber()))
            || (a == SymbolType.STRING || b == SymbolType.STRING) 
            && (a != SymbolType.VOID && b != SymbolType.VOID);

    }

    public static bool IsNumber(this SymbolType a) 
    {
        return a == SymbolType.FLOAT
            || a == SymbolType.INT
            || a == SymbolType.LONG;
    }
    
    public static SymbolType GetTypeNumberResult(this SymbolType a, SymbolType b) 
    {
        if(a == SymbolType.FLOAT || b == SymbolType.FLOAT)
            return SymbolType.FLOAT;
        else if(a == SymbolType.LONG || b == SymbolType.LONG)
            return SymbolType.LONG;
        else if(a == SymbolType.INT || b == SymbolType.INT)
            return SymbolType.INT;
        else 
            return a;

    }
}