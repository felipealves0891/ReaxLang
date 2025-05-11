using System;
using Reax.Core.Types;
using Reax.Parser;
using Reax.Parser.Node;
using Reax.Parser.Node.Literals;
using Reax.Parser.Node.Operations;

namespace Reax.Lexer;

public static class TokenExtensions
{
    public static bool IsReaxValue(this Token token)
    {
        return token.Type switch 
        {
            TokenType.IDENTIFIER => true,
            TokenType.STRING_LITERAL => true,
            TokenType.NUMBER_LITERAL => true,
            TokenType.TRUE => true,
            TokenType.FALSE => true,
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
        DataType dataType;
        if(type is null)
            dataType = DataType.NONE;
        else
            dataType = type.Value.Type.ToDataType();

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
