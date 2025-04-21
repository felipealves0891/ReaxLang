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
        var block = (ContextNode)source.NextBlock();

        var node = new FunctionDeclarationNode(identifier, block, parameters, typeReturn, identifier.Location);
        Logger.LogParse(node.ToString());
        return node;
    }
    
    private IEnumerable<VarNode> GetParameters(ITokenSource source) 
    {
        var parameters = new List<VarNode>();
        if(source.CurrentToken.Type != TokenType.START_PARAMETER)
            return parameters;

        source.Advance();
        while(source.CurrentToken.Type != TokenType.END_PARAMETER) 
        {
            if(source.CurrentToken.Type == TokenType.IDENTIFIER)
            {
                var value = source.CurrentToken;
                source.Advance();    
                source.Advance();
                var type = source.CurrentToken;
                parameters.Add((VarNode)value.ToReaxValue(type));
            }
            
            source.Advance();
        }
        
        source.Advance();
        return parameters;
    }
}
