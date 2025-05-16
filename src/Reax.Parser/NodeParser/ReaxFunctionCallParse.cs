using System;
using Reax.Core.Debugger;
using Reax.Lexer;
using Reax.Parser.Extensions;
using Reax.Parser.Node;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast;

namespace Reax.Parser.NodeParser;

public class ReaxFunctionCallParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.IDENTIFIER 
            && next.Type == TokenType.OPEN_PARENTHESIS;
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
            else if (statement.Type == TokenType.OPEN_PARENTHESIS)
                startParameter = true;
            else if (statement.Type == TokenType.CLOSE_PARENTHESIS)
                break;
        }

        if(identifier is null)
            throw new Exception();
        
        var textIdentifier = identifier.Value.Source;
        var values = parameter.Select(x => x.ToReaxValue()).ToArray();

        return new FunctionCallNode(textIdentifier, values, identifier.Value.Location);
    }
}
