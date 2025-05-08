using Reax.Debugger;
using Reax.Lexer.Reader;
using Reax.Parser;

namespace Reax.Lexer;

public class ReaxLexer 
{
    private readonly IReader _source;

    public ReaxLexer(IReader source)
    {
        _source = source;
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
            return new Token(TokenType.EOF, (byte)' ', _source.FileName, _source.Position, _source.Line);

        if(char.IsWhiteSpace((char)_source.CurrentChar)) 
            _source.Advance();
        if(char.IsLetter((char)_source.CurrentChar)) 
            return GetIdentifierOrKeyword();
        if(char.IsDigit((char)_source.CurrentChar)) 
            return GetDigit();
        if(_source.CurrentChar == '\'') 
            return GetString();
        if(_source.CurrentChar == '(') 
            return AdvanceAndReturn(new Token(TokenType.START_PARAMETER, (byte)'(', _source.FileName, _source.Column, _source.Line));   
        if(_source.CurrentChar == ',') 
            return AdvanceAndReturn(new Token(TokenType.PARAMETER_SEPARATOR, (byte)',', _source.FileName, _source.Column, _source.Line));   
        if(_source.CurrentChar == ')') 
            return AdvanceAndReturn(new Token(TokenType.END_PARAMETER, (byte)')', _source.FileName, _source.Column, _source.Line));       
        if(_source.CurrentChar == ';') 
            return AdvanceAndReturn(new Token(TokenType.END_STATEMENT, (byte)';', _source.FileName, _source.Column, _source.Line));   
        if(IsLetterOrIsDigitOrWhiteSpace(_source.BeforeChar) && _source.CurrentChar == '=' && IsLetterOrIsDigitOrWhiteSpace(_source.NextChar)) 
            return AdvanceAndReturn(new Token(TokenType.ASSIGNMENT, (byte)'=', _source.FileName, _source.Column, _source.Line));   
        if(_source.BeforeChar == '=' && _source.CurrentChar == '=') 
            return AdvanceAndReturn(new Token(TokenType.EQUALITY, new byte[] {(byte)'=', (byte)'='}, _source.FileName, _source.Column, _source.Line));   
        if(_source.CurrentChar == '!' && _source.NextChar == '=') 
            return AdvanceAndReturn(new Token(TokenType.EQUALITY, new byte[] {(byte)'!', (byte)'='}, _source.FileName, _source.Column, _source.Line));   
        if(_source.CurrentChar == '-' && _source.NextChar == '>')
            return GetArrow();
        if(_source.CurrentChar == '<' || _source.CurrentChar == '>') 
            return GetComparison();
        if(_source.CurrentChar == '+' || _source.CurrentChar == '-') 
            return AdvanceAndReturn(new Token(TokenType.TERM, _source.CurrentChar, _source.FileName, _source.Column, _source.Line));   
        if(_source.CurrentChar == '*' || _source.CurrentChar == '/')
            return AdvanceAndReturn(new Token(TokenType.FACTOR, _source.CurrentChar, _source.FileName, _source.Column, _source.Line));   
        if(_source.CurrentChar == '!')
            return AdvanceAndReturn(new Token(TokenType.NOT, _source.CurrentChar, _source.FileName, _source.Column, _source.Line)); 
        if(_source.CurrentChar == '{')
            return AdvanceAndReturn(new Token(TokenType.START_BLOCK, _source.CurrentChar, _source.FileName, _source.Column, _source.Line)); 
        if(_source.CurrentChar == '}')
            return AdvanceAndReturn(new Token(TokenType.END_BLOCK, _source.CurrentChar, _source.FileName, _source.Column, _source.Line));
        if(_source.CurrentChar == '.')
            return AdvanceAndReturn(new Token(TokenType.ACCESS, _source.CurrentChar, _source.FileName, _source.Column, _source.Line));
        if(_source.CurrentChar == ':')
            return AdvanceAndReturn(new Token(TokenType.TYPING, _source.CurrentChar, _source.FileName, _source.Column, _source.Line));
        if(_source.CurrentChar == '|')
            return AdvanceAndReturn(new Token(TokenType.PIPE, _source.CurrentChar, _source.FileName, _source.Column, _source.Line));
        if(_source.CurrentChar == '#')
            return Comment();
        
        _source.Advance();
        return NextToken();
    }

    private Token Comment() 
    {
        _source.Advance();
        while (_source.CurrentChar != '\n')
            _source.Advance();
        
         _source.Advance();
        return NextToken();
    }

    private Token AdvanceAndReturn(Token token) 
    {
        Logger.LogLexer(token.ToString());
        _source.Advance();
        return token;
    }

    public Token GetDigit() 
    {
        var start = _source.Position;
        Position positionStart = new (_source.Line, _source.Column);
        while(!_source.EndOfFile && (char.IsDigit((char)_source.CurrentChar) || _source.CurrentChar == '.'))
            _source.Advance();

        var number = _source.GetString(start, _source.Position);
        var positionEnd = new Position(_source.Line, _source.Column);
        var token = new Token(TokenType.NUMBER_LITERAL, number, _source.FileName, positionStart, positionEnd);
        Logger.LogLexer(token.ToString());
        return token;
    }

    private Token GetIdentifierOrKeyword() 
    {
        var start = _source.Position;
        Position positionStart = new (_source.Line, _source.Column);

        while(!_source.EndOfFile && IsIdentifier(_source.CurrentChar))
            _source.Advance();

        var identifier = _source.GetString(start,_source.Position);
        var type = Keywords.IsKeyword(identifier);
        var positionEnd = new Position(_source.Line, _source.Column);
        var token = new Token(type, identifier, _source.FileName, positionStart, positionEnd);
        Logger.LogLexer(token.ToString());        
        return token;
    }

    private Token GetString() 
    {
        _source.Advance();
        var start = _source.Position;
        Position positionStart = new (_source.Line, _source.Column);

        while (!_source.EndOfFile && _source.CurrentChar != '\'')
            _source.Advance();
        
        var end = _source.Position;
        var positionEnd = new Position(_source.Line, _source.Column);
        var text = _source.GetString(start, end);
        _source.Advance();

        var token = new Token(TokenType.STRING_LITERAL, text, _source.FileName, positionStart, positionEnd);
        Logger.LogLexer(token.ToString());
        return token;
    }

    private Token GetComparison() 
    {
        var start = _source.Position;
        Position positionStart = new (_source.Line, _source.Column);
        Position positionEnd;

        _source.Advance();
        if(_source.BeforeChar != '=')
        {
            positionEnd = new Position(_source.Line, _source.Column);
            return new Token(TokenType.COMPARISON, _source.BeforeChar, _source.FileName, positionStart, positionEnd);
        }
        
        var end = _source.Position;        
        positionEnd = new Position(_source.Line, _source.Column);
        _source.Advance();

        var token = new Token(TokenType.COMPARISON, _source.GetString(start, end), _source.FileName, positionStart, positionEnd);
        Logger.LogLexer(token.ToString());
        return token;
    }

    private Token GetArrow() 
    {
        var start = new Position(_source.Line, _source.Column);
        _source.Advance();
        _source.Advance();
        var end = new Position(_source.Line, _source.Column);
        var token = new Token(TokenType.ARROW, new byte[] {(byte)'-', (byte)'>'}, _source.FileName, start, end);
        Logger.LogLexer(token.ToString());
        return token;
    }

    public bool IsIdentifier(byte b) 
        => char.IsLetter((char)b) || char.IsDigit((char)b) || (char)b == '_';

    private bool IsLetterOrIsDigitOrWhiteSpace(byte c) 
    {
        return char.IsLetter((char)c) || char.IsDigit((char)c) || char.IsWhiteSpace((char)c);
    }   
}