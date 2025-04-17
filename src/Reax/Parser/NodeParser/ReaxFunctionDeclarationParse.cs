using System;
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
        var block = source.NextBlock();
        return new FunctionNode(identifier, block, parameters);
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
                parameters.Add(source.CurrentToken.ToReaxValue());

            source.Advance();
        }
        
        source.Advance();
        return parameters;
    }
}
