using System;

namespace Reax.Lexer;

public class ReaxLexer 
{
    private readonly string _source;
    private int _numberOfRows;
    private int _position;

    public ReaxLexer(string source)
    {
        _source = source;
        _position = 0;
        _numberOfRows = 0;
    }

    public bool EndOfFile => _position >= _source.Length;
    public char BeforeChar => _source[_position-1];
    public char CurrentChar => _source[_position];
    public char NextChar => _source[_position+1];

    public IEnumerable<Token> Tokenize() 
    {
        Token token;
        do
        {   
            token = NextToken();
            yield return token;
        } 
        while(token.Type != TokenType.EOF);
    }

    public Token NextToken() 
    {
        if(EndOfFile)
            return new Token(TokenType.EOF, "", _position, _numberOfRows);

        if(char.IsWhiteSpace(CurrentChar)) 
            Advance();
        if(char.IsLetter(CurrentChar)) 
            return GetIdentifierOrKeyword();
        if(char.IsDigit(CurrentChar)) 
            return GetDigit();
        if(CurrentChar == '\'') 
            return GetString();
        if(CurrentChar == '(') 
            return AdvanceAndReturn(new Token(TokenType.START_PARAMETER, "(", _position, _numberOfRows));   
        if(CurrentChar == ',') 
            return AdvanceAndReturn(new Token(TokenType.PARAMETER_SEPARATOR, ",", _position, _numberOfRows));   
        if(CurrentChar == ')') 
            return AdvanceAndReturn(new Token(TokenType.END_PARAMETER, ")", _position, _numberOfRows));       
        if(CurrentChar == ';') 
            return AdvanceAndReturn(new Token(TokenType.END_STATEMENT, ";", _position, _numberOfRows));   
        if(IsLetterOrIsDigitOrWhiteSpace(BeforeChar) && CurrentChar == '=' && IsLetterOrIsDigitOrWhiteSpace(NextChar)) 
            return AdvanceAndReturn(new Token(TokenType.ASSIGNMENT, "=", _position, _numberOfRows));   
        if(BeforeChar == '=' && CurrentChar == '=') 
            return AdvanceAndReturn(new Token(TokenType.EQUALITY, "==", _position, _numberOfRows));   
        if(CurrentChar == '!' && NextChar == '=') 
            return AdvanceAndReturn(new Token(TokenType.EQUALITY, "!=", _position, _numberOfRows));   
        if(CurrentChar == '-' && NextChar == '>')
            return GetArrow();
        if(CurrentChar == '<' || CurrentChar == '>') 
            return GetComparison();
        if(CurrentChar == '+' || CurrentChar == '-') 
            return AdvanceAndReturn(new Token(TokenType.TERM, CurrentChar.ToString(), _position, _numberOfRows));   
        if(CurrentChar == '*' || CurrentChar == '/')
            return AdvanceAndReturn(new Token(TokenType.FACTOR, CurrentChar.ToString(), _position, _numberOfRows));   
        if(CurrentChar == '!')
            return AdvanceAndReturn(new Token(TokenType.UNARY, CurrentChar.ToString(), _position, _numberOfRows)); 
        if(CurrentChar == '{')
            return AdvanceAndReturn(new Token(TokenType.START_BLOCK, CurrentChar.ToString(), _position, _numberOfRows)); 
        if(CurrentChar == '}')
            return AdvanceAndReturn(new Token(TokenType.END_BLOCK, CurrentChar.ToString(), _position, _numberOfRows));
        if(CurrentChar == '\n')
            _numberOfRows++;
        
        Advance();
        return NextToken();
    }

    private Token AdvanceAndReturn(Token token) 
    {
        Advance();
        return token;
    }

    public Token GetDigit() 
    {
        var start = _position;
        while(!EndOfFile && (char.IsDigit(CurrentChar) || CurrentChar == '.'))
            Advance();

        var number = _source[start.._position];
        return new Token(TokenType.NUMBER, number, _position, _numberOfRows);
    }

    private Token GetIdentifierOrKeyword() 
    {
        var start = _position;
        while(!EndOfFile && IsIdentifier(CurrentChar))
            Advance();

        var identifier = _source[start.._position];
        return identifier switch 
        {
            "let" => new Token(TokenType.LET, identifier, start, _numberOfRows),
            "if" =>  new Token(TokenType.IF, identifier, start, _numberOfRows),
            "else" => new Token(TokenType.ELSE, identifier, start, _numberOfRows),
            "on" =>  new Token(TokenType.ON, identifier, start, _numberOfRows),
            "true" =>  new Token(TokenType.TRUE, identifier, start, _numberOfRows),
            "false" => new Token(TokenType.FALSE, identifier, start, _numberOfRows),
            "fun" => new Token(TokenType.FUNCTION, identifier, start, _numberOfRows),
            "return" => new Token(TokenType.RETURN, identifier, start, _numberOfRows),
            "when" =>  new Token(TokenType.WHEN, identifier, start, _numberOfRows),
            "for" =>  new Token(TokenType.FOR, identifier, start, _numberOfRows),
            "to" =>  new Token(TokenType.TO, identifier, start, _numberOfRows),
            "while" =>  new Token(TokenType.WHILE, identifier, start, _numberOfRows),
            "and" =>  new Token(TokenType.AND, identifier, start, _numberOfRows),
            "or" =>  new Token(TokenType.OR, identifier, start, _numberOfRows),
            "not" =>  new Token(TokenType.NOT, identifier, start, _numberOfRows),
            "import" =>  new Token(TokenType.IMPORT, identifier, start, _numberOfRows),
            _ => new Token(TokenType.IDENTIFIER, identifier, start, _numberOfRows)
        };
    }

    private Token GetString() 
    {
        Advance();
        var start = _position;
        while (!EndOfFile && CurrentChar != '\'')
            Advance();
        
        var end = _position;
        var text = _source[start..end];
        Advance();
        return new Token(TokenType.STRING, text, _position, _numberOfRows);
    }

    private Token GetComparison() 
    {
        var start = _position;
        _position++;
        if(_source[_position] != '=')
            return new Token(TokenType.COMPARISON, _source[start].ToString(), _position, _numberOfRows);
        
        var end = _position;
        Advance();
        return new Token(TokenType.COMPARISON, _source[start..end], _position, _numberOfRows);
    }

    private Token GetArrow() 
    {
        var start = _position;
        Advance();
        Advance();
        return new Token(TokenType.ARROW, "->", start, _numberOfRows);
    }

    private void Advance() 
    {
        if(EndOfFile)
            throw new InvalidOperationException("Não é possivel avançar após o fim do arquivo!");
        _position++;
    }

    public bool IsIdentifier(char c) 
        => char.IsLetter(c) || char.IsDigit(c) || c == '_';

    private bool IsLetterOrIsDigitOrWhiteSpace(char c) 
    {
        return char.IsLetter(c) || char.IsDigit(c) || char.IsWhiteSpace(c);
    }   
}