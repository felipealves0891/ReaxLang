using Reax.Core.Debugger;
using Reax.Lexer;
using Reax.Lexer.Reader;
using System.Linq;

namespace Reax.Tests.Lexer;

public class ReaxLexerTest : BaseTest<ReaxLexer>
{
    private IReader _mockedReader;

    public ReaxLexerTest()
    {
        _mockedReader = new ReaxTextReader("", "");
    }
    
    public static IEnumerable<object[]> Tokenize_HappyWay_Data 
    {
        get
        {
            return [
                ["meuNumero * 5 + 2;", new TokenType[] {TokenType.IDENTIFIER, TokenType.FACTOR, TokenType.NUMBER_LITERAL, TokenType.TERM, TokenType.NUMBER_LITERAL, TokenType.SEMICOLON, TokenType.EOF}],
                ["console.writer(minha_variavel, opcao);", new TokenType[] {TokenType.IDENTIFIER, TokenType.ACCESS, TokenType.IDENTIFIER, TokenType.OPEN_PARENTHESIS, TokenType.IDENTIFIER, TokenType.COMMA, TokenType.IDENTIFIER, TokenType.CLOSE_PARENTHESIS, TokenType.SEMICOLON, TokenType.EOF}],
                ["minha_variavel = 'Meu texto alterado ';", new TokenType[] {TokenType.IDENTIFIER, TokenType.ASSIGNMENT, TokenType.STRING_LITERAL, TokenType.SEMICOLON, TokenType.EOF}],
                ["bind meuNumeroVezes2: float -> meuNumero * 2;", new TokenType[] {TokenType.BIND, TokenType.IDENTIFIER, TokenType.COLON, TokenType.FLOAT_TYPE, TokenType.ARROW, TokenType.IDENTIFIER, TokenType.FACTOR, TokenType.NUMBER_LITERAL, TokenType.SEMICOLON, TokenType.EOF}],
                ["const meuValorImutavel: string = 'Sempre assim';", new TokenType[] {TokenType.CONST, TokenType.IDENTIFIER, TokenType.COLON, TokenType.STRING_TYPE, TokenType.ASSIGNMENT, TokenType.STRING_LITERAL, TokenType.SEMICOLON, TokenType.EOF}],
                ["let minhaDivisao: int;", new TokenType[] {TokenType.LET, TokenType.IDENTIFIER, TokenType.COLON, TokenType.INT_TYPE, TokenType.SEMICOLON, TokenType.EOF}],
                ["for controle: int = 0 to 100 {}", new TokenType[] {TokenType.FOR, TokenType.IDENTIFIER, TokenType.COLON, TokenType.INT_TYPE, TokenType.ASSIGNMENT, TokenType.NUMBER_LITERAL, TokenType.TO, TokenType.NUMBER_LITERAL, TokenType.OPEN_BRACE, TokenType.CLOSE_BRACE, TokenType.EOF}],
                ["while controle < 100 {}", new TokenType[] {TokenType.WHILE, TokenType.IDENTIFIER, TokenType.COMPARISON, TokenType.NUMBER_LITERAL, TokenType.OPEN_BRACE, TokenType.CLOSE_BRACE, TokenType.EOF}],
                ["fun eMenorQueZero(num: int):bool | string {}", new TokenType[] {TokenType.FUNCTION, TokenType.IDENTIFIER, TokenType.OPEN_PARENTHESIS, TokenType.IDENTIFIER, TokenType.COLON, TokenType.INT_TYPE, TokenType.CLOSE_PARENTHESIS, TokenType.COLON, TokenType.BOOLEAN_TYPE, TokenType.PIPE, TokenType.STRING_TYPE, TokenType.OPEN_BRACE, TokenType.CLOSE_BRACE, TokenType.EOF}],
                ["on minha_variavel -> console.writer('Minha variavel foi alterada3!');", new TokenType[] {TokenType.ON, TokenType.IDENTIFIER, TokenType.ARROW, TokenType.IDENTIFIER, TokenType.ACCESS, TokenType.IDENTIFIER, TokenType.OPEN_PARENTHESIS, TokenType.STRING_LITERAL, TokenType.CLOSE_PARENTHESIS, TokenType.SEMICOLON, TokenType.EOF}],
                ["'texto 1' == 'texto 2'", new TokenType[] {TokenType.STRING_LITERAL, TokenType.EQUALITY, TokenType.STRING_LITERAL, TokenType.EOF}],
                ["'texto 1' != 'texto 2'", new TokenType[] {TokenType.STRING_LITERAL, TokenType.EQUALITY, TokenType.STRING_LITERAL, TokenType.EOF}],
                ["!access", new TokenType[] {TokenType.NOT, TokenType.IDENTIFIER, TokenType.EOF}],
                ["# apenas um comentario", new TokenType[] {TokenType.EOF}],
                ["# apenas um comentario \n return value;", new TokenType[] {TokenType.RETURN, TokenType.IDENTIFIER, TokenType.SEMICOLON, TokenType.EOF}],
                ["5 >= 1", new TokenType[] {TokenType.NUMBER_LITERAL, TokenType.COMPARISON, TokenType.NUMBER_LITERAL, TokenType.EOF}],
                ["5 => 1", new TokenType[] {TokenType.NUMBER_LITERAL, TokenType.COMPARISON, TokenType.NUMBER_LITERAL, TokenType.EOF}],
                ["[5, 10];", new TokenType[] { TokenType.OPEN_BRACKET, TokenType.NUMBER_LITERAL, TokenType.COMMA, TokenType.NUMBER_LITERAL, TokenType.CLOSE_BRACKET, TokenType.SEMICOLON, TokenType.EOF }],
                ["\n\n\n\n", new TokenType[] {TokenType.EOF}],
                ["for controle: int in [0]", new TokenType[] {TokenType.FOR, TokenType.IDENTIFIER, TokenType.COLON, TokenType.INT_TYPE, TokenType.IN, TokenType.OPEN_BRACKET, TokenType.NUMBER_LITERAL, TokenType.CLOSE_BRACKET, TokenType.EOF}],
                ["struct Lang { name: string }", new TokenType[] {TokenType.STRUCT, TokenType.IDENTIFIER, TokenType.OPEN_BRACE, TokenType.IDENTIFIER, TokenType.COLON, TokenType.STRING_TYPE, TokenType.CLOSE_BRACE, TokenType.EOF}],
                ["let p: @Pessoal;", new TokenType[] {TokenType.LET, TokenType.IDENTIFIER, TokenType.COLON, TokenType.AT, TokenType.IDENTIFIER, TokenType.SEMICOLON, TokenType.EOF}],
            ];
        }
    }

    [Theory]
    [MemberData(nameof(Tokenize_HappyWay_Data))]
    public void Tokenize_HappyWay(string code, IEnumerable<TokenType> expected) 
    {
        //Arrange
        _mockedReader = new ReaxTextReader(code, "source.reax");

        //Act
        var tokens = CreateTested().Tokenize().Select(x => x.Type).ToArray();

        //Assert
        Assert.NotNull(tokens);
        Assert.True(expected.SequenceEqual(tokens));
    }

    protected override ReaxLexer CreateTested()
    {
        Logger.Enabled = false;
        return new ReaxLexer(_mockedReader);
    }
}
