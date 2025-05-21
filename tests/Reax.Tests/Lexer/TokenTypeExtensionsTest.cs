using Reax.Core.Types;
using Reax.Lexer;
using Reax.Parser;

namespace Reax.Tests.Lexer;

public class TokenTypeExtensionsTest
{
    [Theory]
    [InlineData(TokenType.BOOLEAN_TYPE, DataType.BOOLEAN)]
    [InlineData(TokenType.NUMBER_TYPE, DataType.NUMBER)]
    [InlineData(TokenType.STRING_TYPE, DataType.STRING)]
    [InlineData(TokenType.VOID_TYPE, DataType.VOID)]
    public void ToDataType_ShouldReturnCorrectDataType(TokenType tokenType, DataType expectedDataType)
    {
        Assert.Equal(expectedDataType, tokenType.ToDataType());
    }

    [Fact]
    public void ToDataType_InvalidType_ShouldThrowInvalidDataException()
    {
        var invalidTokenType = TokenType.EOF; // Simulando um tipo inválido
        Assert.Throws<InvalidDataException>(() => invalidTokenType.ToDataType());
    }
}
