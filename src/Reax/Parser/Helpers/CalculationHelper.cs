using System;
using Reax.Lexer;
using Reax.Parser.Node;

namespace Reax.Parser.Helpers;
//CalculationHelper

class CalculationHelper
{
    private List<Token> _tokens;
    private int _pos = 0;

    public CalculationHelper(IEnumerable<Token> tokens) => _tokens = tokens.ToList();

    private Token? Peek() => _pos < _tokens.Count ? _tokens[_pos] : new Token(TokenType.EOF, "", -1, -1);
    private Token Consume() => _tokens[_pos++];

    public ReaxNode ParseExpression() => ParseAddSub();

    private ReaxNode ParseAddSub()
    {
        var node = ParseMulDiv();
        while (Peek()?.Source is "+" or "-")
        {
            var op = Consume();
            var right = ParseMulDiv();
            node = new CalculateNode(node, op.ToArithmeticOperator(), right);
        }
        return node;
    }

    private ReaxNode ParseMulDiv()
    {
        var node = ParseFactor();
        while (Peek()?.Source is "*" or "/")
        {
            var op = Consume();
            var right = ParseFactor();
            node = new CalculateNode(node, op.ToArithmeticOperator(), right);
        }
        return node;
    }

    private ReaxNode ParseFactor()
    {
        var token = Peek();
        if (token is not null && token.IsReaxValue())
        {
            Consume();
            return token.ToReaxValue();
        }
        else if (token is not null && token.Type == TokenType.START_PARAMETER)
        {
            Consume();
            var node = ParseExpression();
            if (Peek()?.Type != TokenType.END_PARAMETER) throw new Exception("Esperado ')'");
            Consume();
            return node;
        }
        else
        {
            throw new Exception($"Token inesperado: {token}");
        }
    }
}