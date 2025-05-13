using System.Text;
using Reax.Core.Locations;

namespace Reax.Lexer;

public struct Token
{
    public static TokenType[] DataTypes = [
        TokenType.BOOLEAN_TYPE,
        TokenType.FLOAT_TYPE,
        TokenType.INT_TYPE,
        TokenType.LONG_TYPE, 
        TokenType.STRING_TYPE, 
        TokenType.VOID_TYPE
    ];

    private readonly SourceLocation _location;
    private readonly byte[] _source;

    public Token(TokenType type, string source, string file, Position start, Position end)
    {
        _source = Encoding.UTF8.GetBytes(source);
        _location = new SourceLocation(file, start, end);
        Type = type;
    }


    public Token(TokenType type, byte[] source, string file, Position start, Position end)
    {
        _source = source;
        _location = new SourceLocation(file, start, end);
        Type = type;
    }

    public Token(TokenType type, byte source, string file, Position start, Position end)
    {
        _source = new byte[] { source };
        _location = new SourceLocation(file, start, end);
        Type = type;
    }
    
    public Token(TokenType type, byte source, string file, int column, int line)
    {
        _source = new byte[] { source };
        _location = new SourceLocation(file, new Position(line, column), new Position(line, column));
        Type = type;
    }
    
    public Token(TokenType type, byte[] source, string file, int column, int line)
    {
        _source = source;
        _location = new SourceLocation(file, new Position(line, column), new Position(line, column));
        Type = type;
    }

    public TokenType Type { get; init; }
    public int Position => _location.Start.Column;
    public int Row  => _location.Start.Line;
    public string File => _location.File;
    public ReadOnlySpan<byte> ReadOnlySource => new ReadOnlySpan<byte>(_source);
    public string Source => Encoding.GetEncoding("utf-8").GetString(_source);
    public SourceLocation Location => _location;

    public Token AppendAtBeginning(Token token) 
    {
        var newSource = new byte[_source.Length + token.ReadOnlySource.Length];
        var position = 0;

        for (int i = 0; i < token.ReadOnlySource.Length; i++)
        {
            newSource[position] = token.ReadOnlySource[i];
            position++;
        }

        for (int i = 0; i < _source.Length; i++)
        {
            newSource[position] = _source[i];
            position++;
        }

        return new Token(Type, newSource, token.Location.File, token.Location.Start, token.Location.End);
    }

    public override string ToString()
    {
        return $"{File}#{Row} {Source} is {Type} at {Position}";
    }
}