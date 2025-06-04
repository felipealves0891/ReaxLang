using System;
using Reax.Core.Ast;
using Reax.Core.Ast.Expressions;
using Reax.Core.Locations;
using Reax.Lexer;

namespace Reax.Parser.NodeParser;

public class InvokeParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.INVOKE;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        var keyword = source.CurrentToken;
        source.Advance(TokenType.IDENTIFIER);
        var identifier = source.CurrentToken;
        source.Advance(TokenType.SEMICOLON);
        source.Advance();
        return new InvokeNode(identifier.Source,
            new SourceLocation(
                keyword.File,
                keyword.Location.Start,
                identifier.Location.End));
    }
}
