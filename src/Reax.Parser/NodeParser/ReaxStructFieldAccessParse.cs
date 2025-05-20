using System;
using Reax.Core.Ast;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Objects.Structs;
using Reax.Core.Locations;
using Reax.Lexer;
using Reax.Parser.Helper;

namespace Reax.Parser.NodeParser;

public class ReaxStructFieldAccessParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return before.Type == TokenType.ASSIGNMENT
            && current.Type == TokenType.IDENTIFIER
            && next.Type == TokenType.ARROW;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        var instance = source.CurrentToken;
        source.Advance(TokenType.ARROW);
        source.Advance(TokenType.IDENTIFIER);
        var property = source.CurrentToken;

        var structFieldAccessNode
            = new StructFieldAccessNode(
                instance.Source,
                property.Source,
                new SourceLocation(
                    instance.File,
                    instance.Location.Start,
                    property.Location.End));
        
        if (source.NextToken.Type == TokenType.OPEN_BRACKET)
        {
            source.Advance(TokenType.OPEN_BRACKET);
            source.Advance();
            var array = ExpressionHelper.Parser(GetExpressionTokens(source));
            source.Advance(TokenType.SEMICOLON);
            source.Advance();
            return new ArrayAccessNode(structFieldAccessNode, array, structFieldAccessNode.Location);
        }

        source.Advance(TokenType.SEMICOLON);
        source.Advance();
        return structFieldAccessNode;

    }

    private IEnumerable<Token> GetExpressionTokens(ITokenSource source)
    {
        do
        {
            yield return source.CurrentToken;
            source.Advance();
        }
        while (source.CurrentToken.Type != TokenType.CLOSE_BRACKET);
    }
}
