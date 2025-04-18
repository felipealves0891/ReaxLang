using System.Text;
using Reax.Parser.Node;

namespace Reax.Lexer;

public struct Token
{
    private readonly byte[] _source;

    public Token(TokenType type, byte[] source, int position, int row)
    {
        Type = type;
        _source = source;
        Position = position;
        Row = row;
    }

    public Token(TokenType type, byte source, int position, int row)
    {
        Type = type;
        _source = new byte[] { source };
        Position = position;
        Row = row;
    }

    public TokenType Type { get; init; }
    public int Position { get; init; }
    public int Row { get; init; }
    public ReadOnlySpan<byte> ReadOnlySource => new ReadOnlySpan<byte>(_source);
    public string Source => Encoding.GetEncoding("utf-8").GetString(_source);

    public override string ToString()
    {
        return $"#{Row} {Source} is {Type} at {Position}";
    }
}

public static class TokenExtensions
{
    public static bool IsReaxValue(this Token token)
    {
        return token.Type switch 
        {
            TokenType.IDENTIFIER => true,
            TokenType.STRING => true,
            TokenType.NUMBER => true,
            _ => false
        };
    }

    public static bool CanCalculated(this Token token)
    {
        return token.Type switch 
        {
            TokenType.IDENTIFIER => true,
            TokenType.NUMBER => true,
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
            TokenType.TERM => new TermNode(token.Source),
            TokenType.FACTOR => new FactorNode(token.Source.ToString()),
            _ => throw new InvalidOperationException($"Não é possivel converter {token.Type} em operador aritimetico!")
        };
    }

    public static ReaxNode ToReaxValue(this Token token) 
    {
        return token.Type switch 
        {
            TokenType.IDENTIFIER => new VarNode(token.Source),
            TokenType.STRING => new StringNode(token.Source),
            TokenType.NUMBER => new NumberNode(token.Source),
            TokenType.FALSE => new BooleanNode(token.Source),
            TokenType.TRUE => new BooleanNode(token.Source),
            _ => throw new InvalidOperationException($"Não é possivel converter {token.Type} em valor!")
        };
    }

    public static ReaxNode ToLogicOperator(this Token token) 
    {
        return token.Type switch 
        {
            TokenType.COMPARISON => new ComparisonNode(token.Source),
            TokenType.EQUALITY => new EqualityNode(token.Source),
            TokenType.AND => new LogicNode(token.Source),
            TokenType.OR => new LogicNode(token.Source),
            TokenType.NOT => new LogicNode(token.Source),
            _ => throw new InvalidOperationException($"Não é possivel converter {token.Type} em valor!")
        };
    }
}
