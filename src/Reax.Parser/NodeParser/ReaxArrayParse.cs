using System;
using System.Collections.Immutable;
using Reax.Core.Ast;
using Reax.Core.Ast.Literals;
using Reax.Core.Ast.Objects;
using Reax.Lexer;
using Reax.Parser.Extensions;

namespace Reax.Parser.NodeParser;

public class ReaxArrayParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return before.Type == TokenType.ASSIGNMENT
            && current.Type == TokenType.OPEN_BRACKET;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        var nodes = new List<ReaxNode>();
        var assign = source.BeforeToken;

        do
        {
            source.Advance([TokenType.NUMBER_LITERAL, TokenType.STRING_LITERAL, TokenType.IDENTIFIER]);
            var node = source.CurrentToken.ToReaxValue();
            nodes.Add(node);
            source.Advance([TokenType.COMMA, TokenType.CLOSE_BRACKET]);
        }
        while (source.CurrentToken.Type != TokenType.CLOSE_BRACKET);

        source.Advance(TokenType.SEMICOLON);
        source.Advance();

        return new ArrayNode(nodes.ToImmutableArray(), assign.Location);
    }
}
