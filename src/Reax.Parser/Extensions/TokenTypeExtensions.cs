using Reax.Core.Types;

namespace Reax.Lexer;

public static class TokenTypeExtensions
{
    public static DataType ToDataType(this TokenType type)
    {
        return type switch 
        {
            TokenType.BOOLEAN_TYPE => DataType.BOOLEAN,
            TokenType.NUMBER_TYPE => DataType.NUMBER,
            TokenType.STRING_TYPE => DataType.STRING,
            TokenType.VOID_TYPE => DataType.VOID,
            TokenType.AT => DataType.STRUCT,
            _ => throw new InvalidDataException("Tipo de dado invalido:")
        };
    }
}