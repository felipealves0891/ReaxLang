using System;
using Reax.Debugger;
using Reax.Lexer;
using Reax.Parser.Node;

namespace Reax.Parser.NodeParser;

public class ReaxDeclarationParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.LET;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        Token? identifier = null;
        Token? value = null;

        foreach (var statement in source.NextStatement())
        {
            if(statement.Type == TokenType.IDENTIFIER)
                identifier = statement;
            else if (statement.IsReaxValue())
                value = statement;
        }

        if(identifier is null)
            throw new Exception();

        var textIdentifier = identifier.Value.Source;
        ReaxNode node;
        if(value is not null)
            node = new DeclarationNode(textIdentifier, value.Value.ToReaxValue());
        else 
            node = new DeclarationNode(textIdentifier, null);

        Logger.LogParse(node.ToString());
        return node;
    }
}
