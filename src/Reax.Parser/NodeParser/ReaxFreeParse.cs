using System;
using Reax.Core.Ast;
using Reax.Core.Ast.Statements;
using Reax.Lexer;

namespace Reax.Parser.NodeParser;

public class ReaxFreeParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.FREE;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        var freeToken = source.CurrentToken;
        source.Advance(TokenType.IDENTIFIER);
        var identifier = source.CurrentToken;
        source.Advance(TokenType.SEMICOLON);
        source.Advance();
        return new FreeNode(identifier.Source, freeToken.Location);
    }
}
