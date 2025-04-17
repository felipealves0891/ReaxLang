using System;
using Reax.Lexer.Reader;

namespace Reax.Lexer;

public class ReaxLexer 
{
    private readonly IReader _source;
    private int _numberOfRows;

    public ReaxLexer(IReader source)
    {
        _source = source;
        _numberOfRows = 0;
    }

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
        if(_source.EndOfFile)
            return new Token(TokenType.EOF, ' ', _source.Position, _numberOfRows);

        if(char.IsWhiteSpace(_source.CurrentChar)) 
            _source.Advance();
        if(char.IsLetter(_source.CurrentChar)) 
            return GetIdentifierOrKeyword();
        if(char.IsDigit(_source.CurrentChar)) 
            return GetDigit();
        if(_source.CurrentChar == '\'') 
            return GetString();
        if(_source.CurrentChar == '(') 
            return AdvanceAndReturn(new Token(TokenType.START_PARAMETER, '(', _source.Position, _numberOfRows));   
        if(_source.CurrentChar == ',') 
            return AdvanceAndReturn(new Token(TokenType.PARAMETER_SEPARATOR, ',', _source.Position, _numberOfRows));   
        if(_source.CurrentChar == ')') 
            return AdvanceAndReturn(new Token(TokenType.END_PARAMETER, ')', _source.Position, _numberOfRows));       
        if(_source.CurrentChar == ';') 
            return AdvanceAndReturn(new Token(TokenType.END_STATEMENT, ';', _source.Position, _numberOfRows));   
        if(IsLetterOrIsDigitOrWhiteSpace(_source.BeforeChar) && _source.CurrentChar == '=' && IsLetterOrIsDigitOrWhiteSpace(_source.NextChar)) 
            return AdvanceAndReturn(new Token(TokenType.ASSIGNMENT, '=', _source.Position, _numberOfRows));   
        if(_source.BeforeChar == '=' && _source.CurrentChar == '=') 
            return AdvanceAndReturn(new Token(TokenType.EQUALITY, new char[] {'=', '='}, _source.Position, _numberOfRows));   
        if(_source.CurrentChar == '!' && _source.NextChar == '=') 
            return AdvanceAndReturn(new Token(TokenType.EQUALITY, new char[] {'!', '='}, _source.Position, _numberOfRows));   
        if(_source.CurrentChar == '-' && _source.NextChar == '>')
            return GetArrow();
        if(_source.CurrentChar == '<' || _source.CurrentChar == '>') 
            return GetComparison();
        if(_source.CurrentChar == '+' || _source.CurrentChar == '-') 
            return AdvanceAndReturn(new Token(TokenType.TERM, _source.CurrentChar, _source.Position, _numberOfRows));   
        if(_source.CurrentChar == '*' || _source.CurrentChar == '/')
            return AdvanceAndReturn(new Token(TokenType.FACTOR, _source.CurrentChar, _source.Position, _numberOfRows));   
        if(_source.CurrentChar == '!')
            return AdvanceAndReturn(new Token(TokenType.UNARY, _source.CurrentChar, _source.Position, _numberOfRows)); 
        if(_source.CurrentChar == '{')
            return AdvanceAndReturn(new Token(TokenType.START_BLOCK, _source.CurrentChar, _source.Position, _numberOfRows)); 
        if(_source.CurrentChar == '}')
            return AdvanceAndReturn(new Token(TokenType.END_BLOCK, _source.CurrentChar, _source.Position, _numberOfRows));
        if(_source.CurrentChar == '.')
            return AdvanceAndReturn(new Token(TokenType.ACCESS, _source.CurrentChar, _source.Position, _numberOfRows));
        if(_source.CurrentChar == '\n')
            _numberOfRows++;
        
        _source.Advance();
        return NextToken();
    }

    private Token AdvanceAndReturn(Token token) 
    {
        _source.Advance();
        return token;
    }

    public Token GetDigit() 
    {
        var start = _source.Position;
        while(!_source.EndOfFile && (char.IsDigit(_source.CurrentChar) || _source.CurrentChar == '.'))
            _source.Advance();

        var number = _source.GetString(start, _source.Position);
        return new Token(TokenType.NUMBER, number, _source.Position, _numberOfRows);
    }

    private Token GetIdentifierOrKeyword() 
    {
        var start = _source.Position;
        while(!_source.EndOfFile && IsIdentifier(_source.CurrentChar))
            _source.Advance();

        var identifier = _source.GetString(start,_source.Position);
        var type = Keywords.IsKeyword(identifier);
        return new Token(type, identifier, start, _numberOfRows);
    }

    private Token GetString() 
    {
        _source.Advance();
        var start = _source.Position;
        while (!_source.EndOfFile && _source.CurrentChar != '\'')
            _source.Advance();
        
        var end = _source.Position;
        var text = _source.GetString(start, end);
        _source.Advance();
        return new Token(TokenType.STRING, text, _source.Position, _numberOfRows);
    }

    private Token GetComparison() 
    {
        var start = _source.Position;
        _source.Advance();
        if(_source.BeforeChar != '=')
            return new Token(TokenType.COMPARISON, _source.BeforeChar, _source.Position, _numberOfRows);
        
        var end = _source.Position;
        _source.Advance();
        return new Token(TokenType.COMPARISON, _source.GetString(start, end), _source.Position, _numberOfRows);
    }

    private Token GetArrow() 
    {
        var start = _source.Position;
        _source.Advance();
        _source.Advance();
        return new Token(TokenType.ARROW, new char[] {'-', '>'}, start, _numberOfRows);
    }

    public bool IsIdentifier(char c) 
        => char.IsLetter(c) || char.IsDigit(c) || c == '_';

    private bool IsLetterOrIsDigitOrWhiteSpace(char c) 
    {
        return char.IsLetter(c) || char.IsDigit(c) || char.IsWhiteSpace(c);
    }   
}