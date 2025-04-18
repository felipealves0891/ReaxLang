using System;
using Reax.Lexer;
using Reax.Parser.Node;

namespace Reax.Parser.NodeParser;

public class ReaxAssignmentParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.IDENTIFIER 
            && next.Type == TokenType.ASSIGNMENT;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        Token? identifier = null;
        bool isExpression = false;
        List<Token> expression = new List<Token>();
        ReaxNode? value = null;
        
        foreach (var statement in source.NextStatement())
        {
            if(!isExpression && statement.Type == TokenType.IDENTIFIER)
                identifier = statement;
            else if (!isExpression && statement.Type == TokenType.ASSIGNMENT)
                isExpression = true;
            else if (isExpression)
                expression.Add(statement);
        }

        if(identifier is null || expression is null)
            throw new Exception();
        
        if(expression.Count() == 1)
        {
            value = expression[0].ToReaxValue();
        }
        else 
        {
            var parser = new ReaxParser(expression);
            var expressionNodes = parser.Parse();
            value = new ContextNode(expressionNodes.ToArray());
        }

        return new AssignmentNode(identifier.Value.Source, value);
    }
}
