using System;
using Reax.Debugger;
using Reax.Lexer;
using Reax.Parser.Helper;
using Reax.Parser.Node;
using Reax.Parser.Node.Expressions;
using Reax.Parser.Node.Statements;

namespace Reax.Parser.NodeParser;

public class ReaxWhileParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.WHILE;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        source.Advance();
        var statement = source.NextStatement().ToArray();
        var condition = (BinaryNode)ExpressionHelper.Parser(statement);
        var block = (ContextNode)source.NextBlock();
        return new WhileNode(condition, block, condition.Location);
    }
}
