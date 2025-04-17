using System;
using Reax.Lexer;
using Reax.Parser.Node;

namespace Reax.Parser.NodeParser;

public class ReaxModuleFunctionCallParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.IDENTIFIER && next.Type == TokenType.ACCESS;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        var moduleName = source.CurrentToken;
        source.Advance();
        source.Advance();
        var identifier = source.CurrentToken;
        source.Advance();
        if(source.CurrentToken.Type != TokenType.START_PARAMETER)
            throw new InvalidOperationException($"Era esperado um abre parenteses '(' na linha {source.CurrentToken.Row}!");
        
        source.Advance();
        var parameters = new List<ReaxNode>();
        while(source.CurrentToken.Type != TokenType.END_PARAMETER)
        {
            if(source.CurrentToken.IsReaxValue())
                parameters.Add(source.CurrentToken.ToReaxValue());
            else if(source.CurrentToken.Type != TokenType.PARAMETER_SEPARATOR)
                throw new InvalidOperationException($"Token invalido na linha {source.CurrentToken.Row}.");

            source.Advance();
        }
        source.Advance();

        return new ModuleFunctionCallNode(
            moduleName.ReadOnlySource.ToString(), 
            new FunctionCallNode(identifier.ReadOnlySource.ToString(), parameters.ToArray()));
    }
}
