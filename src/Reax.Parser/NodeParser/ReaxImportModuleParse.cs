using System;
using Reax.Core.Debugger;
using Reax.Lexer;
using Reax.Parser.Node;
using Reax.Core.Ast.Statements;
using Reax.Core.Ast;
using Reax.Core.Registries;

namespace Reax.Parser.NodeParser;

public class ReaxImportModuleParse : INodeParser
{
    private static BuiltInRegistry _builtInRegistry = new ();

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
        
        if(source.CurrentToken.Type != TokenType.SEMICOLON)
            throw new InvalidOperationException("Era esperado o encerramento da expressão!");

        source.Advance();
        var functions = _builtInRegistry.Get(identifier.Source);        
        var node = new ModuleNode(identifier.Source, functions, identifier.Location);
        return node;
    }
}
