using System;
using Reax.Debugger;
using Reax.Lexer;
using Reax.Parser.Node;

namespace Reax.Parser.NodeParser;

public class ReaxDeclarationParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.LET || current.Type == TokenType.CONST
           || (current.Type == TokenType.ASYNC && next.Type == TokenType.LET);
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        Token? identifier = null;
        List<Token> values = new List<Token>();
        Token? dataType = null;
        
        var isAsync = source.CurrentToken.Type == TokenType.ASYNC;
        if(isAsync) source.Advance();

        var immutable = source.CurrentToken.Type == TokenType.CONST;
        var isTyping = false;

        var isAssignment = false;

        foreach (var statement in source.NextStatement())
        {
            if(statement.Type == TokenType.IDENTIFIER && !identifier.HasValue)
                identifier = statement;
            else if(statement.Type == TokenType.TYPING)
                isTyping = true;
            else if(isTyping && !dataType.HasValue)
                dataType = statement;    
            else if (statement.Type == TokenType.ASSIGNMENT)
                isAssignment = true;
            else if(isAssignment)
                values.Add(statement);
        }

        if(immutable && !values.Any())
            throw new InvalidOperationException("A constante deve ser definida na declaração!");

        if(identifier is null)
            throw new InvalidOperationException("Era esperado um identificar!");

        if(dataType is null)
            throw new InvalidOperationException("Era esperado o tipo da variavel!");

        var type = new DataTypeNode(dataType.Value.Source, dataType.Value.Location);
        var textIdentifier = identifier.Value.Source;
        ReaxNode node;
        if(values.Count == 1)
            node = new DeclarationNode(textIdentifier, immutable, isAsync, type, values.First().ToReaxValue(), identifier.Value.Location);
        else if(values.Any())
        {
            var parser = new ReaxParser(values);
            var context = new ContextNode(parser.Parse().ToArray(), identifier.Value.Location);
            node = new DeclarationNode(textIdentifier, immutable, isAsync, type, context, identifier.Value.Location);
        }
        else 
            node = new DeclarationNode(textIdentifier, immutable,  isAsync, type, null, identifier.Value.Location);

        Logger.LogParse(node.ToString());
        return node;
    }
}
