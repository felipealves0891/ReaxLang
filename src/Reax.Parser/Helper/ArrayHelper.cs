using System;
using System.Collections.Immutable;
using Reax.Core.Ast;
using Reax.Core.Ast.Objects;
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
}
