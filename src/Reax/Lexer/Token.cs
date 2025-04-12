using Reax.Parser.Node;

namespace Reax.Lexer;

public record Token(TokenType Type, string Source, int Position)
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

    public static ReaxNode ToReaxValue(this Token token) 
    {
        return token.Type switch 
        {
            TokenType.IDENTIFIER => new VarNode(token.Source),
            TokenType.STRING => new StringNode(token.Source),
            TokenType.NUMBER => new NumberNode(token.Source),
            _ => throw new InvalidOperationException($"Não é possivel converter {token.Type} em valor!")
        };
    }
}
