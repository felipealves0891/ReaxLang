using System;
using System.Linq.Expressions;
using Reax.Debugger;
using Reax.Lexer;
using Reax.Parser.Helper;
using Reax.Parser.Node;

namespace Reax.Parser.NodeParser;

public class ReaxReturnHandleParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.RETURN 
           && (next.Type == TokenType.SUCCESS || next.Type == TokenType.ERROR);
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        var location = source.CurrentToken.Location;
        source.Advance([TokenType.SUCCESS, TokenType.ERROR]);
        var resultBranch = source.CurrentToken.Type;
        source.Advance();
        var statement = source.NextStatement().ToArray();

        ReaxNode result;
        if(statement.Length == 1)
        {
            result = CreateNode(resultBranch, statement[0].ToReaxValue(), location);
        }
        else 
        {
            var node = new ContextNode([ExpressionHelper.Parser(statement)], location);
            result = CreateNode(resultBranch, node, location);
        }
        
        Logger.LogParse(result.ToString());
        return result;
    }

    private ReaxNode CreateNode(
        TokenType resultBranch, 
        ReaxNode expression, 
        SourceLocation location) 
    {
        if(resultBranch == TokenType.SUCCESS)
            return new ReturnSuccessNode(expression, location);
        else
            return new ReturnErrorNode(expression, location);
    }
}
