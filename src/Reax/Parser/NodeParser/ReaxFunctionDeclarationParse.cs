using System;
using Reax.Debugger;
using Reax.Lexer;
using Reax.Parser.Node;

namespace Reax.Parser.NodeParser;

public class ReaxFunctionDeclarationParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.FUNCTION;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        source.Advance();
        var identifier = source.CurrentToken.ToReaxValue();
        source.Advance();
        var parameters = GetParameters(source).ToArray();
        source.Advance();
        var typeReturn = new DataTypeNode(source.CurrentToken.Source, source.CurrentToken.Location);
        source.Advance();
        var block = source.NextBlock();

        var node = new FunctionNode(identifier, block, parameters, typeReturn, identifier.Location);
        Logger.LogParse(node.ToString());
        return node;
    }
    
    private IEnumerable<ReaxNode> GetParameters(ITokenSource source) 
    {
        var parameters = new List<ReaxNode>();
        if(source.CurrentToken.Type != TokenType.START_PARAMETER)
            return parameters;

        source.Advance();
        while(source.CurrentToken.Type != TokenType.END_PARAMETER) 
        {
            if(source.CurrentToken.Type == TokenType.IDENTIFIER)
            {
                var value = source.CurrentToken.ToReaxValue();
                var identifier = source.CurrentToken.Source;
                source.Advance();    
                source.Advance();
                var type = source.CurrentToken.Source;
                parameters.Add(value);
            }
            source.Advance();
        }
        
        source.Advance();
        return parameters;
    }
}
