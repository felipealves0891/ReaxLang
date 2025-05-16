using System;
using Moq;
using Reax.Lexer;
using Reax.Parser.Helper;
using Reax.Parser.NodeParser;

namespace Reax.Tests.Parser.Helper;

public class ParameterHelperTest : BaseTest<ParameterHelper>
{
    [Fact]
    public void GetParameters_HappyWay() 
    {
        //Arrange
        var _mockedTokenSource = CreateTokenSource([
            new Token(TokenType.OPEN_PARENTHESIS, (byte)' ', "", 0, 0),
            new Token(TokenType.IDENTIFIER, (byte)'m', "", 0, 0),
            new Token(TokenType.IDENTIFIER, (byte)'m', "", 0, 0),
            new Token(TokenType.INT_TYPE, (byte)' ', "", 0, 0),
            new Token(TokenType.CLOSE_PARENTHESIS, (byte)' ', "", 0, 0),
        ]);

        //Act
        var parameters = ParameterHelper.GetParameters(_mockedTokenSource);

        //Assert
        Assert.NotEmpty(parameters);
        Assert.Single(parameters);
        var single = parameters.Single();
        Assert.Equal("m", single.Identifier);
    }

    protected override ParameterHelper CreateTested()
    {
        throw new NotImplementedException();
    }
}
