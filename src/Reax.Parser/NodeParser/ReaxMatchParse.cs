using System;
using Reax.Lexer;
using Reax.Parser.Helper;
using Reax.Parser.Node;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Statements;
using Reax.Core.Ast;

namespace Reax.Parser.NodeParser;

public class ReaxMatchParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.MATCH;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        var location = source.CurrentToken.Location;
        source.Advance();
        ReaxNode expression = ExpressionHelper.Parser(source.NextStatement());
        
        ActionNode? success = null;
        ActionNode? error = null;

        source.Advance([TokenType.SUCCESS, TokenType.ERROR]);
        for (int i = 0; i < 2; i++)
        {            
            if (source.CurrentToken.Type == TokenType.SUCCESS)
                success = ParseFunction(source);
            else if (source.CurrentToken.Type == TokenType.ERROR)
                error = ParseFunction(source);
            else
                throw new InvalidOperationException($"{location} - Token invalido! Era espera sucesso ou erro!");
        }

        source.Advance();
        if(success is null || error is null)
            throw new InvalidOperationException($"{location} - Token invalido! Era espera sucesso ou erro!");

        return new MatchNode((ExpressionNode)expression, success, error, location);
    }

    private ActionNode ParseFunction(ITokenSource source) 
    {
        var location = source.CurrentToken.Location;
        source.Advance(TokenType.START_PARAMETER);
        var parameters = ParameterHelper.GetParameters(source).First();
        source.Advance(Token.DataTypes);
        var dataType = source.CurrentToken.Type.ToDataType();
        source.Advance([TokenType.ARROW, TokenType.START_BLOCK]);
        ReaxNode? expression = null;
        if(source.CurrentToken.Type == TokenType.ARROW)
        {
            source.Advance();
            var value = ExpressionHelper.Parser(source.NextStatement());
            expression = new ReturnSuccessNode(value, value.Location);
        }
        else if(source.CurrentToken.Type == TokenType.START_BLOCK)
            expression = source.NextBlock();

        if(expression is null)
            throw new InvalidOperationException($"{location} - Era aguardado uma expressão!");

        return new ActionNode(parameters, expression, dataType, location);
    }
}
