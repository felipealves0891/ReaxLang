using System;
using System.Linq.Expressions;
using Reax.Core.Debugger;
using Reax.Lexer;
using Reax.Parser.Extensions;
using Reax.Parser.Helper;
using Reax.Parser.Node;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Statements;
using Reax.Core.Ast;
using Reax.Parser.NodeParser;

namespace Reax.Core.AstParser;

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
        var statement = source.NextExpression().ToArray();

        ReaxNode result;
        
        if(statement.Length == 1)
        {
            result = new ReturnSuccessNode(statement[0].ToReaxValue(), location);
        }
        else 
        {
            result = new ReturnSuccessNode(ExpressionHelper.Parser(statement), location);
        }
        
        return result;
    }
}
