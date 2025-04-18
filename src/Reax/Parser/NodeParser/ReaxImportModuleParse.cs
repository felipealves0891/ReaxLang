using System;
using Reax.Lexer;
using Reax.Parser.Node;

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
        var identifier = source.CurrentToken.Source;
        source.Advance();
        
        if(source.CurrentToken.Type != TokenType.END_STATEMENT)
            throw new InvalidOperationException("Era esperado o encerramento da expressão!");

        source.Advance();
        var functions = ReaxEnvironment.BuiltInRegistry.Get(identifier);
        return new ModuleNode(identifier, functions);
    }
}
