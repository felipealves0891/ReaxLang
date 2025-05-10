using System;
using Reax.Lexer;
using Reax.Parser.Node;
using Reax.Parser.Node.Literals;
using Reax.Parser.Node.Operations;

namespace Reax.Tests.Lexer;

public class TokenExtensionsTests
{
    [Theory]
    [InlineData(TokenType.IDENTIFIER, true)]
    [InlineData(TokenType.STRING_LITERAL, true)]
    [InlineData(TokenType.NUMBER_LITERAL, true)]
    [InlineData(TokenType.TRUE, true)]
    [InlineData(TokenType.FALSE, true)]
    [InlineData(TokenType.COMPARISON, false)]
    public void IsReaxValue_ShouldReturnCorrectResult(TokenType type, bool expected)
    {
        var token = new Token { Type = type };
        Assert.Equal(expected, token.IsReaxValue());
    }

    [Theory]
    [InlineData(TokenType.IDENTIFIER, true)]
    [InlineData(TokenType.NUMBER_LITERAL, true)]
    [InlineData(TokenType.STRING_LITERAL, false)]
    public void CanCalculated_ShouldReturnCorrectResult(TokenType type, bool expected)
    {
        var token = new Token { Type = type };
        Assert.Equal(expected, token.CanCalculated());
    }

    [Theory]
    [InlineData(TokenType.TERM, true)]
    [InlineData(TokenType.FACTOR, true)]
    [InlineData(TokenType.IDENTIFIER, false)]
    public void IsArithmeticOperator_ShouldReturnCorrectResult(TokenType type, bool expected)
    {
        var token = new Token { Type = type };
        Assert.Equal(expected, token.IsArithmeticOperator());
    }
    [Theory]
    [InlineData(TokenType.TERM, typeof(TermNode))]
    [InlineData(TokenType.FACTOR, typeof(FactorNode))]
    public void ToArithmeticOperator_ShouldReturnCorrectNodeType(TokenType type, Type expectedType)
    {
        var token = new Token(type, 0, "", 0, 1);
        var result = token.ToArithmeticOperator();
        Assert.IsType(expectedType, result);
    }

    [Theory]
    [InlineData(TokenType.IDENTIFIER, typeof(VarNode))]
    [InlineData(TokenType.STRING_LITERAL, typeof(StringNode))]
    [InlineData(TokenType.NUMBER_LITERAL, typeof(NumberNode))]
    [InlineData(TokenType.FALSE, typeof(BooleanNode))]
    [InlineData(TokenType.TRUE, typeof(BooleanNode))]
    public void ToReaxValue_ShouldReturnCorrectNodeType(TokenType type, Type expectedType)
    {
        var token = new Token(type, 0, "", 0, 1);
        var result = token.ToReaxValue();
        Assert.IsType(expectedType, result);
    }

    [Theory]
    [InlineData(TokenType.COMPARISON, typeof(ComparisonNode))]
    [InlineData(TokenType.EQUALITY, typeof(EqualityNode))]
    [InlineData(TokenType.AND, typeof(LogicNode))]
    [InlineData(TokenType.OR, typeof(LogicNode))]
    [InlineData(TokenType.NOT, typeof(LogicNode))]
    public void ToLogicOperator_ShouldReturnCorrectNodeType(TokenType type, Type expectedType)
    {
        var token = new Token(type, 0, "", 0, 1);
        var result = token.ToLogicOperator();
        Assert.IsType(expectedType, result);
    }
}