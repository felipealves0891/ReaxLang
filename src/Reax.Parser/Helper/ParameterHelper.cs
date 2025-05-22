using System;
using Reax.Core.Ast;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Interfaces;
using Reax.Lexer;
using Reax.Parser.Extensions;
using Reax.Parser.Node;
using Reax.Parser.NodeParser;

namespace Reax.Parser.Helper;

public class ParameterHelper
{
    public static IEnumerable<VarNode> GetParameters(ITokenSource source)
    {
        var parameters = new List<VarNode>();
        if (source.CurrentToken.Type != TokenType.OPEN_PARENTHESIS)
            return parameters;

        source.Advance();
        while (source.CurrentToken.Type != TokenType.CLOSE_PARENTHESIS)
        {
            if (source.CurrentToken.Type == TokenType.IDENTIFIER)
            {
                var value = source.CurrentToken;
                source.Advance();
                source.Advance();
                var type = source.CurrentToken;
                parameters.Add((VarNode)value.ToReaxValue(type));
            }

            source.Advance();
        }

        source.Advance();
        return parameters;
    }

    public static IEnumerable<ReaxNode> GetCallParameters(ITokenSource source, bool validateEndExpression = true)
    {
        var parameters = new List<ReaxNode>();
        if (source.CurrentToken.Type != TokenType.OPEN_PARENTHESIS)
            return parameters;

        source.Advance();
        TokenType[] allowType = [TokenType.IDENTIFIER, TokenType.TRUE, TokenType.FALSE, TokenType.NUMBER_LITERAL, TokenType.STRING_LITERAL];
        while (source.CurrentToken.Type != TokenType.CLOSE_PARENTHESIS)
        {
            if (allowType.Contains(source.CurrentToken.Type))
            {
                var tokens = new List<Token>();

                while (source.CurrentToken.Type != TokenType.COMMA && source.CurrentToken.Type != TokenType.CLOSE_PARENTHESIS)
                {
                    tokens.Add(source.CurrentToken);
                    source.Advance();
                }
                
                parameters.Add(ExpressionHelper.Parser(tokens));
            }

            if (source.CurrentToken.Type == TokenType.CLOSE_PARENTHESIS && source.NextToken.Type != TokenType.CLOSE_PARENTHESIS) break;
            source.Advance();
        }

        if (validateEndExpression)
            source.Advance(TokenType.SEMICOLON);
        else 
            source.Advance();
            
        return parameters;
    }
}
