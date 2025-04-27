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
        source.Advance(TokenType.IDENTIFIER);
        var identifier = new IdentifierNode(source.CurrentToken.Source, source.CurrentToken.Location);
        source.Advance(TokenType.START_PARAMETER);
        var parameters = GetParameters(source).ToArray();
        source.Advance(Token.DataTypes);
        var successType = source.CurrentToken.Type.ToDataType();
        source.Advance([TokenType.PIPE, TokenType.START_BLOCK]);

        var errorType = DataType.VOID;
        if(source.CurrentToken.Type == TokenType.PIPE)
        {
            source.Advance(Token.DataTypes);
            errorType = source.CurrentToken.Type.ToDataType();
            source.Advance(TokenType.START_BLOCK);
        }

        var block = (ContextNode)source.NextBlock();
        var node = new FunctionDeclarationNode(identifier, block, parameters, successType, errorType, identifier.Location);
        Logger.LogParse(node.ToString());
        return node;
    }
    
    public static IEnumerable<VarNode> GetParameters(ITokenSource source) 
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
