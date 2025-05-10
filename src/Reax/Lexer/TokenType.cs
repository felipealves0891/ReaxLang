using Reax.Parser;

namespace Reax.Lexer;

public enum TokenType 
{
    EOF = -1,
    UNKNOW,
    END_EXPRESSION,
    IDENTIFIER,
    ASSIGNMENT,
    START_PARAMETER,
    PARAMETER_SEPARATOR,
    END_PARAMETER,
    STRING_LITERAL,
    NUMBER_LITERAL,
    LET,
    COMPARISON,
    EQUALITY,
    IF,
    ELSE,
    START_BLOCK,
    END_BLOCK,
    TERM,
    FACTOR,
    ON,
    TRUE,
    FALSE,
    ARROW,
    FUNCTION,
    RETURN,
    WHEN,
    FOR,
    TO,
    WHILE,
    AND,
    OR,
    NOT,
    IMPORT,
    ACCESS,
    SCRIPT,
    MODULE,
    CONST,
    ASYNC,
    BIND,
    TYPING,
    PIPE,
    SUCCESS,
    ERROR,
    BOOLEAN_TYPE,
    FLOAT_TYPE,
    INT_TYPE,
    LONG_TYPE,
    STRING_TYPE,
    VOID_TYPE,
    MATCH
} 

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
