using System;
using System.Collections.Generic;
using Reax.Lexer;
using Reax.Parser.Helper;
using Reax.Parser.Node.Expressions;
using Xunit;

namespace Reax.Tests.Parser.Helper;

public class ExpressionHelperTests
{
    [Fact]
    public void Parser_ShouldReturnValidReaxNode()
    {
        var tokens = new Token[]
        {
            new Token(TokenType.IDENTIFIER, (byte)'x', "x", 0, 0),
            new Token(TokenType.COMPARISON, (byte)'<', "<", 1, 0),
            new Token(TokenType.NUMBER_LITERAL, (byte)'5', "5", 2, 0)
        };

        var result = ExpressionHelper.Parser(tokens);
        Assert.NotNull(result);
        Assert.IsType<BinaryNode>(result);
    }

    [Fact]
    public void ParserBinary_ShouldReturnBinaryNode()
    {
        var tokens = new Token[]
        {
            new Token(TokenType.IDENTIFIER, (byte)'x', "x", 0, 0),
            new Token(TokenType.EQUALITY, (byte)'=', "==", 1, 0),
            new Token(TokenType.NUMBER_LITERAL, (byte)'5', "5", 2, 0)
        };

        var result = ExpressionHelper.ParserBinary(tokens);
        Assert.NotNull(result);
        Assert.IsType<BinaryNode>(result);
    }

    [Fact]
    public void ParseExpression_ShouldHandleArithmeticOperators()
    {
        var tokens = new Token[]
        {
            new Token(TokenType.NUMBER_LITERAL, (byte)'2', "source.reax", 0, 0),
            new Token(TokenType.TERM, (byte)'+', "source.reax", 1, 0),
            new Token(TokenType.NUMBER_LITERAL, (byte)'3', "source.reax", 2, 0)
        };

        var result = ExpressionHelper.Parser(tokens);
        Assert.NotNull(result);
        Assert.IsType<CalculateNode>(result);
    }

    [Fact]
    public void ParseExpression_ShouldHandleFactors()
    {
        var tokens = new Token[]
        {
            new Token(TokenType.NUMBER_LITERAL, (byte)'2', "source.reax", 0, 0),
            new Token(TokenType.TERM, (byte)'+', "source.reax", 1, 0),
            new Token(TokenType.NUMBER_LITERAL, (byte)'3', "source.reax", 2, 0),
            new Token(TokenType.FACTOR, (byte)'*', "source.reax", 2, 0),
            new Token(TokenType.NUMBER_LITERAL, (byte)'4', "source.reax", 2, 0)
        };

        var result = ExpressionHelper.Parser(tokens);
        Assert.NotNull(result);
        Assert.IsType<CalculateNode>(result);
    }
    
    [Fact]
    public void ParseExpression_ShouldHandlebrackets()
    {
        var tokens = new Token[]
        {
            new Token(TokenType.START_PARAMETER, (byte)'(', "source.reax", 2, 0),
            new Token(TokenType.TERM, (byte)'-', "source.reax", 0, 0),
            new Token(TokenType.NUMBER_LITERAL, [(byte)'2'], "source.reax", 0, 0),
            new Token(TokenType.TERM, (byte)'+', "source.reax", 1, 0),
            new Token(TokenType.NUMBER_LITERAL, (byte)'3', "source.reax", 2, 0),
            new Token(TokenType.END_PARAMETER, (byte)')', "source.reax", 2, 0),
            new Token(TokenType.FACTOR, (byte)'*', "source.reax", 2, 0),
            new Token(TokenType.IDENTIFIER, [(byte)'m'], "source.reax", 2, 0),
            new Token(TokenType.ACCESS, [(byte)'.'], "source.reax", 2, 0),
            new Token(TokenType.IDENTIFIER, [(byte)'m'], "source.reax", 2, 0),
            new Token(TokenType.START_PARAMETER, (byte)'(', "source.reax", 2, 0),
            new Token(TokenType.NUMBER_LITERAL, [(byte)'2'], "source.reax", 0, 0),
            new Token(TokenType.END_PARAMETER, (byte)')', "source.reax", 2, 0),
        };

        var result = ExpressionHelper.Parser(tokens);
        Assert.NotNull(result);
        Assert.IsType<CalculateNode>(result);
    }

    [Fact]
    public void ParseExpression_ShouldThrowOnUnexpectedToken()
    {
        var tokens = new Token[]
        {
            new Token(TokenType.UNKNOW, (byte)'?', "source.reax", 0, 0)
        };

        Assert.Throws<Exception>(() => ExpressionHelper.Parser(tokens));
    }
}