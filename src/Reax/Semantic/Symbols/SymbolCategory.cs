using System;

namespace Reax.Semantic.Symbols;

public enum SymbolCategory
{
    NONE,
    LET,
    CONST,
    FUNCTION,
    PARAMETER,
    PARAMETER_OPTIONAL,
    BIND,
    SCRIPT,
    MODULE
}
