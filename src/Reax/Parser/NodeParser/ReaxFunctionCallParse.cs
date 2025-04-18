using System;
using Reax.Debugger;
using Reax.Lexer;
using Reax.Parser.Node;

namespace Reax.Parser.NodeParser;

public class ReaxFunctionCallParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.IDENTIFIER 
            && next.Type == TokenType.START_PARAMETER;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        bool startParameter = false;

        Token? identifier = null;
        List<Token> parameter = new List<Token>();
        
        foreach (var statement in source.NextStatement())
        {
            if(!startParameter && statement.Type == TokenType.IDENTIFIER)
                identifier = statement;
            else if (startParameter && statement.IsReaxValue())
                parameter.Add(statement);
            else if (statement.Type == TokenType.START_PARAMETER)
                startParameter = true;
        }

        if(identifier is null)
            throw new Exception();
        
        var textIdentifier = identifier.Value.Source;
        var values = parameter.Select(x => x.ToReaxValue()).ToArray();

        var node = new FunctionCallNode(textIdentifier, values);
        Logger.LogParse(node.ToString());
        return node;
    }
}
