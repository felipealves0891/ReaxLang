using System;
using Reax.Lexer;
using Reax.Parser.Helper;
using Reax.Parser.Node;

namespace Reax.Parser.NodeParser;

public class ReaxExpressionParser : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.IsReaxValue() && 
              (next.Type == TokenType.COMPARISON 
            || next.Type == TokenType.EQUALITY
            || next.Type == TokenType.TERM
            || next.Type == TokenType.FACTOR);
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        var statement = source.NextStatement();
        return ExpressionHelper.Parser(statement.ToArray());
    }
}
