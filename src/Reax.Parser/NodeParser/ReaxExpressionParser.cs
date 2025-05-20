using System;
using Reax.Core.Ast;
using Reax.Lexer;
using Reax.Parser.Extensions;
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
        var statement = source.NextExpression();
        return ExpressionHelper.Parser(statement.ToArray());
    }
}
