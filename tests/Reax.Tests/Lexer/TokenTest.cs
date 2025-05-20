using System;
using Reax.Lexer;

namespace Reax.Tests.Lexer;

public class TokenTest
{
    [Fact]
    public void AppendAtBeginning_Join()
    {
        //Arrange
        var token1 = new Token(TokenType.STRING_LITERAL, [(byte)'l', (byte)'a'], "", 1, 1);
        var token2 = new Token(TokenType.STRING_LITERAL, [(byte)'n', (byte)'g'], "", 1, 1);

        //Act
        var newToken = token2.AppendAtBeginning(token1);

        //Assert
        Assert.True(newToken.ReadOnlySource.SequenceEqual([
            (byte)'l', 
            (byte)'a', 
            (byte)'n', 
            (byte)'g']
        ));
    }

    [Fact]
    public void DataTypes_Join()
    {
        //Assert
        Assert.True(Token.DataTypes.SequenceEqual([
        TokenType.BOOLEAN_TYPE,
        TokenType.FLOAT_TYPE,
        TokenType.INT_TYPE,
        TokenType.LONG_TYPE,
        TokenType.STRING_TYPE,
        TokenType.VOID_TYPE,
        TokenType.AT
        ]));
    }
}
