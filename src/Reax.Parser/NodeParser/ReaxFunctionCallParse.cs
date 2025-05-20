using System;
using Reax.Core.Debugger;
using Reax.Lexer;
using Reax.Parser.Extensions;
using Reax.Parser.Node;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast;
using Reax.Parser.Helper;

namespace Reax.Parser.NodeParser;

public class ReaxFunctionCallParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.IDENTIFIER
            && next.Type == TokenType.OPEN_PARENTHESIS;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        return ExpressionHelper.Parser(source.NextExpression());
    }
}
