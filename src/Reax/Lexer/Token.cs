using Reax.Parser.Node;

namespace Reax.Lexer;

public struct Token
{
    private readonly char[] _source;

    public Token(TokenType type, char[] source, int position, int row)
    {
        Type = type;
        _source = source;
        Position = position;
        Row = row;
    }

    public Token(TokenType type, char source, int position, int row)
    {
        Type = type;
        _source = new char[] { source };
        Position = position;
        Row = row;
    }

    public TokenType Type { get; init; }
    public int Position { get; init; }
    public int Row { get; init; }
    public ReadOnlySpan<char> ReadOnlySource => new ReadOnlySpan<char>(_source);

    public override string ToString()
    {
        return $"#{Row} {ReadOnlySource} is {Type} at {Position}";
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
            TokenType.TERM => new TermNode(token.ReadOnlySource.ToString()),
            TokenType.FACTOR => new FactorNode(token.ReadOnlySource.ToString()),
            _ => throw new InvalidOperationException($"Não é possivel converter {token.Type} em operador aritimetico!")
        };
    }

    public static ReaxNode ToReaxValue(this Token token) 
    {
        return token.Type switch 
        {
            TokenType.IDENTIFIER => new VarNode(token.ReadOnlySource.ToString()),
            TokenType.STRING => new StringNode(token.ReadOnlySource.ToString()),
            TokenType.NUMBER => new NumberNode(token.ReadOnlySource.ToString()),
            TokenType.FALSE => new BooleanNode(token.ReadOnlySource.ToString()),
            TokenType.TRUE => new BooleanNode(token.ReadOnlySource.ToString()),
            _ => throw new InvalidOperationException($"Não é possivel converter {token.Type} em valor!")
        };
    }

    public static ReaxNode ToLogicOperator(this Token token) 
    {
        return token.Type switch 
        {
            TokenType.COMPARISON => new ComparisonNode(token.ReadOnlySource.ToString()),
            TokenType.EQUALITY => new EqualityNode(token.ReadOnlySource.ToString()),
            TokenType.AND => new LogicNode(token.ReadOnlySource.ToString()),
            TokenType.OR => new LogicNode(token.ReadOnlySource.ToString()),
            TokenType.NOT => new LogicNode(token.ReadOnlySource.ToString()),
            _ => throw new InvalidOperationException($"Não é possivel converter {token.Type} em valor!")
        };
    }
}
