using Reax.Core.Types;
using Reax.Parser;

namespace Reax.Lexer;

public static class TokenTypeExtensions
{
    public static DataType ToDataType(this TokenType type)
    {
        return type switch 
        {
            TokenType.BOOLEAN_TYPE => DataType.BOOLEAN,
            TokenType.FLOAT_TYPE => DataType.NUMBER,
            TokenType.INT_TYPE => DataType.NUMBER,
            TokenType.LONG_TYPE => DataType.NUMBER,
            TokenType.STRING_TYPE => DataType.STRING,
            TokenType.VOID_TYPE => DataType.VOID,
            _ => throw new InvalidDataException("Tipo de dado invalido:")
        };
    }
}