using System;
using Reax.Lexer;
using Reax.Parser.Node;
using Reax.Runtime.Contexts;

namespace Reax.Parser.Helper;

public static class SymbolHelper
{
    public static void VariableDeclaration(
        Token identifier, 
        bool immutable, 
        bool isAsync, 
        DataType type)
    {
        var category = immutable ? SymbolCategory.CONST : isAsync ? SymbolCategory.LET_ASYNC : SymbolCategory.LET_SYNC;
        var symbol = new Symbol(identifier.Source, type, category, "main", identifier.Location);
        ReaxEnvironment.AnalyzerContext.Declare(symbol);
    }

    public static void VariableDeclarationAndAssign(
        Token identifier, 
        bool immutable, 
        bool isAsync, 
        DataType type, 
        SourceLocation assignLocation)
    {
        var category = immutable ? SymbolCategory.CONST : isAsync ? SymbolCategory.LET_ASYNC : SymbolCategory.LET_SYNC;
        var symbol = new Symbol(identifier.Source, type, category, "main", identifier.Location);
        symbol.MarkAsAssigned(assignLocation);
        ReaxEnvironment.AnalyzerContext.Declare(symbol);
    }
    
    private static Symbol ParameterCreater(VarNode var, string parent)
    {
        return new Symbol(
            var.Identifier, 
            var.Type, 
            SymbolCategory.PARAMETER, 
            parent, 
            var.Location);
    }

    public static void FunctionDeclaration(
        Token identifier, 
        DataType resultType, 
        VarNode[] parameters,
        SourceLocation assignLocation)
    {
        var symbols = parameters.Select(x => ParameterCreater(x, identifier.Source)).ToArray(); 
        var symbol = new Symbol(identifier.Source, resultType, SymbolCategory.FUNCTION, "main", identifier.Location, symbols);
        symbol.MarkAsAssigned(assignLocation);
        ReaxEnvironment.AnalyzerContext.Declare(symbol);
    }
}
