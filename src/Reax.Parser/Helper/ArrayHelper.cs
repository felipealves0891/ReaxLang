using System;
using System.Collections.Immutable;
using Reax.Core.Ast;
using Reax.Core.Ast.Objects;
using Reax.Core.Locations;
using Reax.Lexer;
using Reax.Parser.Extensions;
using Reax.Parser.NodeParser;

namespace Reax.Parser.Helper;

public static class ArrayHelper
{
    public static ReaxNode Parse(ITokenSource source)
    {
        if (source.CurrentToken.Type != TokenType.OPEN_BRACKET)
            return source.CurrentToken.ToReaxValue();

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
        return new ArrayNode(nodes.ToImmutableArray(), assign.Location);
    }

    public static ReaxNode Parse(Token[] tokens)
    {
        int pos = 0;

        if (tokens[pos].Type != TokenType.OPEN_BRACKET)
            return tokens[pos].ToReaxValue();

        var nodes = new List<ReaxNode>();

        do
        {
            if (tokens[++pos].Type is not TokenType.NUMBER_LITERAL and not TokenType.STRING_LITERAL and not TokenType.IDENTIFIER)
                throw new Exception($"Era esperavo um valor, mas foi passado um {tokens[pos + 1].Type}");

            var node = tokens[pos].ToReaxValue();
            nodes.Add(node);
            if (tokens[++pos].Type is not TokenType.COMMA and not TokenType.CLOSE_BRACKET)
                throw new Exception($"Era esperavo um valor, mas foi passado um {tokens[pos + 1].Type}");
        }
        while (tokens[pos].Type != TokenType.CLOSE_BRACKET);
        return new ArrayNode(nodes.ToImmutableArray(),
            new SourceLocation(
                tokens[0].Location.File,
                tokens[0].Location.Start,
                tokens[pos].Location.End));
    }

}
