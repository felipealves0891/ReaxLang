using System;
using Reax.Debugger;
using Reax.Lexer;
using Reax.Parser.Helper;
using Reax.Parser.Node;

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
        var statement = source.NextStatement();
        var helper = new ComparisonHelper(statement);
        var condition = helper.Parse();

        var block = (ContextNode)source.NextBlock();

        var node = new WhileNode(condition, block, condition.Location);
        Logger.LogParse(node.ToString());
        return node;
    }
}
