using System;
using Reax.Debugger;
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
        var location = source.CurrentToken.Location;
        source.Advance();
        var statement = source.NextStatement().ToArray();
        if(statement.Length == 1)
        {
            var result = new ReturnNode(statement[0].ToReaxValue(), statement[0].Location);
            Logger.LogParse(result.ToString());
            return result;
        }
        
        var parser = new ReaxParser(statement);
        var context = parser.Parse();

        var node = new ContextNode(context.ToArray(), statement[0].Location);
        var returnNode = new ReturnNode(node, location);
        Logger.LogParse(returnNode.ToString());

        return returnNode;
    }
}
