using System;
using Reax.Lexer;
using Reax.Parser.Node;

namespace Reax.Parser.NodeParser;

public class ReaxReturnParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.RETURN;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        source.Advance();
        var statement = source.NextStatement().ToArray();
        if(statement.Length == 1)
            return statement[0].ToReaxValue();
        
        var parser = new ReaxParser(statement);
        var context = parser.Parse();
        return new ContextNode(context.ToArray());
    }
}
