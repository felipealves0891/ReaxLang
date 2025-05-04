namespace Reax.Parser;

public enum DataType
{
    NONE = 0,
    STRING = 1 << 0,
    NUMBER = 1 << 1,
    BOOLEAN = 1 << 2,
    NULL = 1 << 3,
    VOID = 1 << 4,
}


public static class DataTypeExtensions
{
    public static bool IsCompatatible(this DataType data, DataType test)
    {
        if(data.HasFlag(test)) return true;
        if(data is DataType.STRING && test is DataType.NUMBER or DataType.BOOLEAN) return true;
        return false;
    }
}
