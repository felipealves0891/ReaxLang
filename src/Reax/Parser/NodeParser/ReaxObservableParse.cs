using System;
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
        var variable = new VarNode(source.CurrentToken.Source);
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
            return new ObservableNode(variable, source.NextBlock(), condition);
        }
        else if(source.CurrentToken.Type == TokenType.ARROW)
        {
            source.Advance();
            var nextNode = source.NextNode() ?? throw new InvalidOperationException();
            return  new ObservableNode(variable, new ContextNode([nextNode]), condition);
        }
        
        throw new InvalidOperationException($"Token invalido '{source.CurrentToken.Type}' na posição: {source.CurrentToken.Row}");
    }
}
