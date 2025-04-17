using System;
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

        var textIdentifier = identifier.Value.ReadOnlySource.ToString();
        if(value is not null)
            return new DeclarationNode(textIdentifier, value.Value.ToReaxValue());
        else 
            return new DeclarationNode(textIdentifier, null);
    }
}
