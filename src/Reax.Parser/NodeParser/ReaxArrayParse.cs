using System;
using System.Collections.Immutable;
using Reax.Core.Ast;
using Reax.Core.Ast.Literals;
using Reax.Core.Ast.Objects;
using Reax.Lexer;
using Reax.Parser.Extensions;
using Reax.Parser.Helper;

namespace Reax.Parser.NodeParser;

public class ReaxArrayParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return (before.Type == TokenType.ASSIGNMENT && current.Type == TokenType.OPEN_BRACKET)
            || (current.Type == TokenType.OPEN_BRACKET && next.Type == TokenType.CLOSE_BRACKET);
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        var array = ArrayHelper.Parse(source);
        source.Advance(TokenType.SEMICOLON);
        source.Advance();
        return array;
    }
}
