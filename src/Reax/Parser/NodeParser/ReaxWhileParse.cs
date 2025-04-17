using System;
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

        var block = source.NextBlock();
        return new WhileNode(condition, block);
    }
}
