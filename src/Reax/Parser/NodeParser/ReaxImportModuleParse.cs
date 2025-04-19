using System;
using Reax.Debugger;
using Reax.Lexer;
using Reax.Parser.Node;
using Reax.Runtime;
using Reax.Runtime.Symbols;

namespace Reax.Parser.NodeParser;

public class ReaxImportModuleParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.IMPORT && next.Type == TokenType.MODULE; 
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        source.Advance();
        source.Advance();
        var identifier = source.CurrentToken;
        source.Advance();
        
        if(source.CurrentToken.Type != TokenType.END_STATEMENT)
            throw new InvalidOperationException("Era esperado o encerramento da expressão!");

        ReaxEnvironment.Symbols.UpdateSymbol(
            identifier.Source,
            Runtime.Symbols.SymbolCategoty.MODULE);

        source.Advance();
        var functions = ReaxEnvironment.BuiltInRegistry.Get(identifier.Source);

        foreach (var key in functions.Keys)
        {
            if(ReaxEnvironment.Symbols.Exists(key))
                ReaxEnvironment.Symbols.UpdateSymbol(key, SymbolCategoty.FUNCTION);
            else
                ReaxEnvironment.Symbols.Set(key, new Symbol(SymbolCategoty.FUNCTION));
        }
        
        var node = new ModuleNode(identifier.Source, functions, identifier.Location);
        Logger.LogParse(node.ToString());
        return node;
    }
}
