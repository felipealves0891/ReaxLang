using System.Text;
using Reax.Parser;
using Reax.Parser.Node;
using Reax.Parser.Node.Literals;
using Reax.Parser.Node.Operations;

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

    public Token(TokenType type, string source, string file, int position, int row)
    {
        _source = Encoding.UTF8.GetBytes(source);
        _location = new SourceLocation(file, row, position);
        Type = type;
    }


    public Token(TokenType type, byte[] source, string file, int position, int row)
    {
        _source = source;
        _location = new SourceLocation(file, row, position);
        Type = type;
    }

    public Token(TokenType type, byte source, string file, int position, int row)
    {
        _source = new byte[] { source };
        _location = new SourceLocation(file, row, position);
        Type = type;
    }

    public TokenType Type { get; init; }
    public int Position => _location.Position;
    public int Row  => _location.Line;
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

        return new Token(Type, newSource, token.Location.File, token.Location.Position, token.Location.Line);

    }

    public override string ToString()
    {
        return $"{File}#{Row} {Source} is {Type} at {Position}";
    }
}

public static class TokenExtensions
{
    public static bool IsReaxValue(this Token token)
    {
        return token.Type switch 
        {
            TokenType.IDENTIFIER => true,
            TokenType.STRING_LITERAL => true,
            TokenType.NUMBER_LITERAL => true,
            _ => false
        };
    }

    public static bool CanCalculated(this Token token)
    {
        return token.Type switch 
        {
            TokenType.IDENTIFIER => true,
            TokenType.NUMBER_LITERAL => true,
            _ => false
        };
    }

    public static bool IsArithmeticOperator(this Token token)
    {
        return token.Type switch 
        {
            TokenType.TERM => true,
            TokenType.FACTOR => true,
            _ => false
        };
    }

    public static ReaxNode ToArithmeticOperator(this Token token) 
    {
        return token.Type switch 
        {
            TokenType.TERM => new TermNode(token.Source, token.Location),
            TokenType.FACTOR => new FactorNode(token.Source.ToString(), token.Location),
            _ => throw new InvalidOperationException($"Não é possivel converter {token.Type} em operador aritimetico!")
        };
    }

    public static ReaxNode ToReaxValue(this Token token, Token? type = null) 
    {
        return token.Type switch 
        {
            TokenType.IDENTIFIER => CreateVar(token, type),
            TokenType.STRING_LITERAL => new StringNode(token.Source, token.Location),
            TokenType.NUMBER_LITERAL => new NumberNode(token.Source, token.Location),
            TokenType.FALSE => new BooleanNode(token.Source, token.Location),
            TokenType.TRUE => new BooleanNode(token.Source, token.Location),
            _ => throw new InvalidOperationException($"Não é possivel converter {token.Type} em valor!")
        };
    }

    private static ReaxNode CreateVar(Token token, Token? type = null) 
    {
        DataTypeNode dataType;
        if(type is null)
            dataType = new DataTypeNode("NONE", token.Location);
        else
            dataType = new DataTypeNode(type.Value.Source, type.Value.Location);

        return new VarNode(
            token.Source, 
            dataType, 
            token.Location);
    }

    public static ReaxNode ToLogicOperator(this Token token) 
    {
        return token.Type switch 
        {
            TokenType.COMPARISON => new ComparisonNode(token.Source, token.Location),
            TokenType.EQUALITY => new EqualityNode(token.Source, token.Location),
            TokenType.AND => new LogicNode(token.Source, token.Location),
            TokenType.OR => new LogicNode(token.Source, token.Location),
            TokenType.NOT => new LogicNode(token.Source, token.Location),
            _ => throw new InvalidOperationException($"Não é possivel converter {token.Type} em valor!")
        };
    }
}
