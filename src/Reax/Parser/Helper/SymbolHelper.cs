using System;
using Reax.Lexer;
using Reax.Runtime.Contexts;

namespace Reax.Parser.Helper;

public static class SymbolHelper
{
    public static void Register(Token identifier, bool immutable, bool isAsync, DataType type)
    {
        var category = immutable ? SymbolCategory.CONST : isAsync ? SymbolCategory.LET_ASYNC : SymbolCategory.LET_SYNC;
        var symbol = new Symbol(identifier.Source, type, category, "main", identifier.Location);
        ReaxEnvironment.AnalyzerContext.Declare(symbol);
    }

    public static void RegisterAndAssign(Token identifier, bool immutable, bool isAsync, DataType type, SourceLocation assignLocation)
    {
        var category = immutable ? SymbolCategory.CONST : isAsync ? SymbolCategory.LET_ASYNC : SymbolCategory.LET_SYNC;
        var symbol = new Symbol(identifier.Source, type, category, "main", identifier.Location);
        symbol.MarkAsAssigned(assignLocation);
        ReaxEnvironment.AnalyzerContext.Declare(symbol);
    }
}
