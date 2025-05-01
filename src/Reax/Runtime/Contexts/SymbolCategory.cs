using System;

namespace Reax.Runtime.Contexts;

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
