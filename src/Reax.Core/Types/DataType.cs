namespace Reax.Core.Types;

public enum DataType
{
    NONE = 0,
    STRING = 1 << 0,
    NUMBER = 1 << 1,
    BOOLEAN = 1 << 2,
    NULL = 1 << 3,
    VOID = 1 << 4,
    ARRAY = 1 << 5

}
