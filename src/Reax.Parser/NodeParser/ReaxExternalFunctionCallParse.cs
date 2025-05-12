using System;
using Reax.Core.Debugger;
using Reax.Lexer;
using Reax.Parser.Extensions;
using Reax.Parser.Node;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast;

namespace Reax.Parser.NodeParser;

public class ReaxExternalFunctionCallParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.IDENTIFIER && next.Type == TokenType.ACCESS;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        var scriptName = source.CurrentToken;
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
            if(source.CurrentToken.Type == TokenType.IDENTIFIER && source.NextToken.Type == TokenType.START_PARAMETER)
                parameters.Add(functionCall(source));
            else if(source.CurrentToken.IsReaxValue())
                parameters.Add(source.CurrentToken.ToReaxValue());
            else if(source.CurrentToken.Type != TokenType.PARAMETER_SEPARATOR)
                throw new InvalidOperationException($"Token invalido '{source.CurrentToken}' na linha {source.CurrentToken.Row}.");

            source.Advance();
        }
        source.Advance();

        if(!source.EndOfTokens)
            source.Advance();

        return new ExternalFunctionCallNode(
            scriptName.Source, 
            new FunctionCallNode(identifier.Source, parameters.ToArray(), identifier.Location),
            identifier.Location);
    }

    private ReaxNode functionCall(ITokenSource source) 
    {
        var location = source.CurrentToken.Location;
        var parser = new ReaxFunctionCallParse();
        return parser.Parse(source) ?? throw new InvalidOperationException($"{location} - Era esperado uma função!");
    }
}
