using System;
using System.Data.Common;
using Reax.Core.Ast;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Objects;
using Reax.Core.Locations;
using Reax.Lexer;
using Reax.Parser.Helper;

namespace Reax.Parser.NodeParser;

public class ReaxStructInstanceParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return (before.Type == TokenType.IDENTIFIER && current.Type == TokenType.ASSIGNMENT && next.Type == TokenType.OPEN_BRACE)
            || (before.Type == TokenType.ASSIGNMENT && current.Type == TokenType.IDENTIFIER && next.Type == TokenType.OPEN_BRACE);
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        Token name = GetName(source);
        source.Advance(TokenType.OPEN_BRACE);

        var fieldValues = new Dictionary<string, ReaxNode>();
        do
        {
            source.Advance(TokenType.IDENTIFIER);
            var identifier = source.CurrentToken;
            source.Advance(TokenType.COLON);
            source.Advance();
            var value = GetValue(source);
            fieldValues.Add(identifier.Source, value);
        }
        while (source.CurrentToken.Type != TokenType.CLOSE_BRACE);

        source.Advance();
        return new StructInstanceNode(
            name.Source,
            fieldValues,
            new SourceLocation(
                name.File,
                name.Location.Start,
                source.CurrentToken.Location.End));
    }

    private Token GetName(ITokenSource source)
    { 
        if (source.CurrentToken.Type == TokenType.IDENTIFIER)
            return source.CurrentToken;
        
        return source.BeforeToken;
    }

    private ReaxNode GetValue(ITokenSource source)
    {
        if (source.CurrentToken.Type != TokenType.OPEN_BRACKET)
            return ExpressionHelper.Parser(NextToComma(source));

        var array = ArrayHelper.Parse(source);
        source.Advance([TokenType.COMMA, TokenType.CLOSE_BRACE]);
        return array;
    }

    private IEnumerable<Token> NextToComma(ITokenSource source)
    {
        while (source.CurrentToken.Type != TokenType.COMMA
            && source.CurrentToken.Type != TokenType.CLOSE_BRACE)
        {
            yield return source.CurrentToken;
            source.Advance();
        }
    }
    
}
