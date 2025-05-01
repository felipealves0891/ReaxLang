using System;
using Reax.Debugger;
using Reax.Lexer;
using Reax.Parser.Helper;
using Reax.Parser.Node;
using Reax.Parser.Node.Statements;

namespace Reax.Parser.NodeParser;

public class ReaxIfNodeParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.IF;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        source.Advance();
        var statement = source.NextStatement().ToArray();
        var condition = ExpressionHelper.ParserBinary(statement);
        var @true = (ContextNode)source.NextBlock();
        ContextNode? @else = null;

        if(source.CurrentToken.Type == TokenType.ELSE)
        {
            source.Advance();
            @else = source.NextBlock() as ContextNode;
        }

        var node = new IfNode(condition, @true, @else, condition.Location);
        Logger.LogParse(node.ToString());
        return node;
    }
}
