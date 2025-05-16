using System;
using Reax.Core.Ast.Expressions;
using Reax.Lexer;
using Reax.Parser.Extensions;
using Reax.Parser.Node;
using Reax.Parser.NodeParser;

namespace Reax.Parser.Helper;

public class ParameterHelper
{
    public static IEnumerable<VarNode> GetParameters(ITokenSource source) 
    {
        var parameters = new List<VarNode>();
        if(source.CurrentToken.Type != TokenType.OPEN_PARENTHESIS)
            return parameters;

        source.Advance();
        while(source.CurrentToken.Type != TokenType.CLOSE_PARENTHESIS) 
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
