using System;
using Reax.Lexer;
using Reax.Parser.Helper;
using Reax.Parser.Node;

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
        var statement = source.NextStatement();
        var comparisonHelper = new ComparisonHelper(statement);
        var condition = comparisonHelper.Parse();
        var @true = source.NextBlock();
        ReaxNode? @else = null;

        if(source.CurrentToken.Type == TokenType.ELSE)
        {
            source.Advance();
            @else = source.NextBlock();
        }

        return new IfNode(condition, @true, @else);
    }
}
