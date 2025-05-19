using System;
using Reax.Core.Debugger;
using Reax.Lexer;
using Reax.Parser.Extensions;
using Reax.Parser.Helper;
using Reax.Parser.Node;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Statements;
using Reax.Core.Ast;

namespace Reax.Parser.NodeParser;

public class ReaxObservableParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.ON;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        source.Advance();
        var variable = (VarNode)source.CurrentToken.ToReaxValue();
        source.Advance();
        
        BinaryNode? condition = null;

        if(source.CurrentToken.Type == TokenType.WHEN)
        {
            source.Advance();    
            var statement = source.NextExpression();
            condition = ExpressionHelper.ParserBinary(statement.ToArray());
        }

        if(source.CurrentToken.Type == TokenType.OPEN_BRACE)
        {
            return new ObservableNode(variable, (ContextNode)source.NextBlock(), condition, source.CurrentToken.Location);
        }
        else if(source.CurrentToken.Type == TokenType.ARROW)
        {
            source.Advance();
            var nextNode = source.NextNode() ?? throw new InvalidOperationException();
            return new ObservableNode(variable, new ContextNode([nextNode], variable.Location), condition, variable.Location);            
        }
        
        throw new InvalidOperationException($"Token invalido '{source.CurrentToken.Type}' na posição: {source.CurrentToken.Row}");
    }
}
