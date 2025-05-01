using System;
using System.Linq.Expressions;
using Reax.Debugger;
using Reax.Lexer;
using Reax.Parser.Helper;
using Reax.Parser.Node;
using Reax.Parser.Node.Expressions;
using Reax.Parser.Node.Statements;

namespace Reax.Parser.NodeParser;

public class ReaxReturnParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.RETURN 
           && next.Type != TokenType.SUCCESS 
           && next.Type != TokenType.ERROR;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        var location = source.CurrentToken.Location;
        source.Advance();
        var statement = source.NextStatement().ToArray();

        ReaxNode result;
        
        if(statement.Length == 1)
        {
            result = new ReturnSuccessNode(statement[0].ToReaxValue(), location);
        }
        else 
        {
            var node = new ContextNode([ExpressionHelper.Parser(statement)], location);
            result = new ReturnSuccessNode(node, location);
        }
        
        return result;
    }
}
