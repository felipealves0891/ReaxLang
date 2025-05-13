using System;

namespace Reax.Core.Types;

public static class DataTypeExtensions
{
    public static bool IsCompatatible(this DataType data, DataType test)
    {
        if(data.HasFlag(test)) return true;
        if(data is DataType.STRING && test is DataType.NUMBER or DataType.BOOLEAN) return true;
        return false;
    }
}
