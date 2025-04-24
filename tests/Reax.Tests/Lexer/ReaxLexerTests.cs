using System;
using Moq;
using Reax.Lexer;
using Reax.Lexer.Reader;

namespace Reax.Tests.Lexer;

public class ReaxLexerTests
{
    private readonly Mock<IReader> _mockedReader;

    public ReaxLexerTests()
    {
        _mockedReader = new Mock<IReader>();
    }

    public static IEnumerable<object[]> Data_Tokenize_ReadStatement
    {
        get
        {
            return [
                [
                    "let x: int = 5;", 
                    new TokenType[] {TokenType.LET, TokenType.IDENTIFIER, TokenType.TYPING, TokenType.INT_TYPE, TokenType.ASSIGNMENT, TokenType.NUMBER_LITERAL, TokenType.END_STATEMENT, TokenType.EOF}
                ],
                [
                    "async let x: int = 5;", 
                    new TokenType[] {TokenType.ASYNC, TokenType.LET, TokenType.IDENTIFIER, TokenType.TYPING, TokenType.INT_TYPE, TokenType.ASSIGNMENT, TokenType.NUMBER_LITERAL, TokenType.END_STATEMENT, TokenType.EOF}
                ],
                [
                    "const x: string = 'text';", 
                    new TokenType[] {TokenType.CONST, TokenType.IDENTIFIER, TokenType.TYPING, TokenType.STRING_TYPE, TokenType.ASSIGNMENT, TokenType.STRING_LITERAL, TokenType.END_STATEMENT, TokenType.EOF}
                ],
                [
                    "console.writer(meuValorImutavel);", 
                    new TokenType[] {TokenType.IDENTIFIER, TokenType.ACCESS, TokenType.IDENTIFIER, TokenType.START_PARAMETER, TokenType.IDENTIFIER, TokenType.END_PARAMETER, TokenType.END_STATEMENT, TokenType.EOF}
                ]
            ];
        }
    }

    [Theory]
    [MemberData(nameof(Data_Tokenize_ReadStatement))]
    public void Tokenize_ReadStatement(string script, TokenType[] expected) 
    {
        //arrange
        var reader = new ReaxTextReader(script);
        var lexer = new ReaxLexer(reader);

        //Act
        var tokens = lexer.Tokenize().Select(x => x.Type).ToArray();

        //Assert
        Assert.Equal(expected.Length, tokens.Length);
        Assert.True(expected.SequenceEqual(tokens));
    }
}
