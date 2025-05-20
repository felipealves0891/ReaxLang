using System;
using Reax.Core.Ast;
using Reax.Lexer;
using Reax.Parser.Helper;

namespace Reax.Parser.NodeParser;

public class ReaxArrayAccessParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return before.Type == TokenType.ASSIGNMENT
            && current.Type == TokenType.IDENTIFIER
            && next.Type == TokenType.OPEN_BRACKET;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        return ExpressionHelper.Parser(source.NextExpression());
    }
}
