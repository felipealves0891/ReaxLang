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
                    "x = 'text';", 
                    new TokenType[] {TokenType.IDENTIFIER, TokenType.ASSIGNMENT, TokenType.STRING_LITERAL, TokenType.END_STATEMENT, TokenType.EOF}
                ],
                [
                    "console.writer(meuValorImutavel);", 
                    new TokenType[] {TokenType.IDENTIFIER, TokenType.ACCESS, TokenType.IDENTIFIER, TokenType.START_PARAMETER, TokenType.IDENTIFIER, TokenType.END_PARAMETER, TokenType.END_STATEMENT, TokenType.EOF}
                ],
                [
                    "bind meuNumeroVezes2: float -> meuNumero * 2;", 
                    new TokenType[] {TokenType.BIND, TokenType.IDENTIFIER, TokenType.TYPING, TokenType.FLOAT_TYPE, TokenType.ARROW, TokenType.IDENTIFIER, TokenType.FACTOR, TokenType.NUMBER_LITERAL, TokenType.END_STATEMENT, TokenType.EOF}
                ],
                [
                    "on minha_variavel { console.writer('Minha variavel foi alterada!'); }", 
                    new TokenType[] {TokenType.ON, TokenType.IDENTIFIER, TokenType.START_BLOCK, TokenType.IDENTIFIER, TokenType.ACCESS, TokenType.IDENTIFIER, TokenType.START_PARAMETER, TokenType.STRING_LITERAL, TokenType.END_PARAMETER, TokenType.END_STATEMENT, TokenType.END_BLOCK, TokenType.EOF}
                ],
                [
                    "on minha_variavel -> console.writer('Minha variavel foi alterada!');", 
                    new TokenType[] {TokenType.ON, TokenType.IDENTIFIER, TokenType.ARROW, TokenType.IDENTIFIER, TokenType.ACCESS, TokenType.IDENTIFIER, TokenType.START_PARAMETER, TokenType.STRING_LITERAL, TokenType.END_PARAMETER, TokenType.END_STATEMENT, TokenType.EOF}
                ],
                [
                    "on minha_variavel when minha_variavel_bool { console.writer('Minha variavel foi alterada!'); }", 
                    new TokenType[] {TokenType.ON, TokenType.IDENTIFIER, TokenType.WHEN, TokenType.IDENTIFIER, TokenType.START_BLOCK, TokenType.IDENTIFIER, TokenType.ACCESS, TokenType.IDENTIFIER, TokenType.START_PARAMETER, TokenType.STRING_LITERAL, TokenType.END_PARAMETER, TokenType.END_STATEMENT, TokenType.END_BLOCK, TokenType.EOF}
                ],
                [
                    "on minha_variavel when minha_variavel > 0 { console.writer('Minha variavel foi alterada!'); }", 
                    new TokenType[] {TokenType.ON, TokenType.IDENTIFIER, TokenType.WHEN, TokenType.IDENTIFIER, TokenType.COMPARISON, TokenType.NUMBER_LITERAL, TokenType.START_BLOCK, TokenType.IDENTIFIER, TokenType.ACCESS, TokenType.IDENTIFIER, TokenType.START_PARAMETER, TokenType.STRING_LITERAL, TokenType.END_PARAMETER, TokenType.END_STATEMENT, TokenType.END_BLOCK, TokenType.EOF}
                ],
                [
                    "if meuNumero > 5 {} else {}", 
                    new TokenType[] {TokenType.IF, TokenType.IDENTIFIER, TokenType.COMPARISON, TokenType.NUMBER_LITERAL, TokenType.START_BLOCK, TokenType.END_BLOCK, TokenType.ELSE, TokenType.START_BLOCK, TokenType.END_BLOCK, TokenType.EOF}
                ],
                [
                    "for controle: int = 0 to 100 {}", 
                    new TokenType[] {TokenType.FOR, TokenType.IDENTIFIER, TokenType.TYPING, TokenType.INT_TYPE, TokenType.ASSIGNMENT, TokenType.NUMBER_LITERAL, TokenType.TO, TokenType.NUMBER_LITERAL, TokenType.START_BLOCK, TokenType.END_BLOCK, TokenType.EOF}
                ],
                [
                    "while controle < 100 {}", 
                    new TokenType[] {TokenType.WHILE, TokenType.IDENTIFIER, TokenType.COMPARISON, TokenType.NUMBER_LITERAL, TokenType.START_BLOCK, TokenType.END_BLOCK, TokenType.EOF}
                ],
                [
                    "fun fazAlgumaCoisa(num: int): void {}", 
                    new TokenType[] {TokenType.FUNCTION, TokenType.IDENTIFIER, TokenType.START_PARAMETER, TokenType.IDENTIFIER, TokenType.TYPING, TokenType.INT_TYPE, TokenType.END_PARAMETER, TokenType.TYPING, TokenType.VOID_TYPE, TokenType.START_BLOCK, TokenType.END_BLOCK, TokenType.EOF}
                ],
                [
                    "import script 'calculate.reax';", 
                    new TokenType[] {TokenType.IMPORT, TokenType.SCRIPT, TokenType.STRING_LITERAL, TokenType.END_STATEMENT, TokenType.EOF}
                ],
                [
                    "import module 'console';", 
                    new TokenType[] {TokenType.IMPORT, TokenType.MODULE, TokenType.STRING_LITERAL, TokenType.END_STATEMENT, TokenType.EOF}
                ],
                [
                    "script calculate;", 
                    new TokenType[] {TokenType.SCRIPT, TokenType.IDENTIFIER, TokenType.END_STATEMENT, TokenType.EOF}
                ]
            ];
        }
    }

    [Theory]
    [MemberData(nameof(Data_Tokenize_ReadStatement))]
    public void Tokenize_ReadStatement(string script, TokenType[] expected) 
    {
        //arrange
        var reader = new ReaxTextReader(script, "");
        var lexer = new ReaxLexer(reader);

        //Act
        var tokens = lexer.Tokenize().Select(x => x.Type).ToArray();

        //Assert
        Assert.Equal(expected.Length, tokens.Length);
        Assert.True(expected.SequenceEqual(tokens));
    }
}
