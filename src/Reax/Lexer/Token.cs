using Reax.Parser.Node;

namespace Reax.Lexer;

public record Token(TokenType Type, string Source, int Position, int Row)
{
    public override string ToString()
    {
        return $"{Source} is {Type} at {Position}";
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
            TokenType.FACTOR => new FactorNode(token.Source),
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
