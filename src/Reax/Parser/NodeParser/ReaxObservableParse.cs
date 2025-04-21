using System;
using Reax.Debugger;
using Reax.Lexer;
using Reax.Parser.Helper;
using Reax.Parser.Node;

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
        var variable = new VarNode(source.CurrentToken.Source, source.CurrentToken.Location);
        source.Advance();
        
        BinaryNode? condition = null;

        if(source.CurrentToken.Type == TokenType.WHEN)
        {
            source.Advance();    
            var statement = source.NextStatement();
            var comparisonHelper = new ComparisonHelper(statement);
            condition = comparisonHelper.Parse();
        }

        if(source.CurrentToken.Type == TokenType.START_BLOCK)
        {
            var node = new ObservableNode(variable, (ContextNode)source.NextBlock(), condition, source.CurrentToken.Location);
            Logger.LogParse(node.ToString());
            return node;
        }
        else if(source.CurrentToken.Type == TokenType.ARROW)
        {
            source.Advance();
            var nextNode = source.NextNode() ?? throw new InvalidOperationException();
            var node = new ObservableNode(variable, new ContextNode([nextNode], variable.Location), condition, variable.Location);
            Logger.LogParse(node.ToString());
            return node;
        }
        
        throw new InvalidOperationException($"Token invalido '{source.CurrentToken.Type}' na posição: {source.CurrentToken.Row}");
    }
}
