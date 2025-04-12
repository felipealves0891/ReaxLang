using System;
using Reax.Lexer;
using Reax.Parser.Node;

namespace Reax.Parser;

public class ReaxParser
{
    private readonly Token[] _tokens;
    private int _position;

    public ReaxParser(IEnumerable<Token> tokens)
    {
        _tokens = tokens.ToArray();
        _position = 0;
    }

    public IEnumerable<ReaxNode> Parse() 
    {
        ReaxNode? node = null;
        do 
        {
            node = NextNode();
            if(node is not null)
                yield return node;
        } 
        while(node is not null);
    }

    public ReaxNode? NextNode() 
    {
        if(_position >= _tokens.Length)
            return null;

        var nextToken = _tokens[_position];
        if(nextToken.Type == TokenType.EOF) 
            return null;
        else if(nextToken.Type == TokenType.LET) 
            return DeclarationParse();
        else if(IsFunctionCall()) 
            return FunctionCallParse();
        else if(IsAssignment()) 
            return AssignmentParse();

        throw new Exception();
    }

    private ReaxNode DeclarationParse() 
    {
        var statements = NextStatement();
        Token? identifier = null;
        Token? value = null;

        foreach (var statement in statements)
        {
            if(statement.Type == TokenType.IDENTIFIER)
                identifier = statement;
            else if (statement.IsReaxValue())
                value = statement;
        }

        if(identifier is null || value is null)
            throw new Exception();

        if(value is not null)
            return new DeclarationNode(identifier.Source, value.ToReaxValue());
        else 
            return new DeclarationNode(identifier.Source, null);
    }

    private bool IsFunctionCall() 
    {
        return _tokens[_position].Type == TokenType.IDENTIFIER 
            && _tokens[_position+1].Type == TokenType.START_PARAMETER;
    }

    private ReaxNode FunctionCallParse() 
    {
        var statements = NextStatement();
        bool startParameter = false;

        Token? identifier = null;
        Token? parameter = null;
        
        foreach (var statement in statements)
        {
            if(!startParameter && statement.Type == TokenType.IDENTIFIER)
                identifier = statement;
            else if (startParameter && statement.IsReaxValue())
                parameter = statement;
            else if (statement.Type == TokenType.START_PARAMETER)
                startParameter = true;
        }

        if(identifier is null || parameter is null)
            throw new Exception();
        
        return new FunctionCallNode(identifier.Source, parameter.ToReaxValue());
    }

    private bool IsAssignment() 
    {
        return _tokens[_position].Type == TokenType.IDENTIFIER 
            && _tokens[_position+1].Type == TokenType.ASSIGNMENT;
    }

    private ReaxNode AssignmentParse() 
    {
        Token? identifier = null;
        Token? value = null;
        
        foreach (var statement in NextStatement())
        {
            if(identifier is null && statement.Type == TokenType.IDENTIFIER)
                identifier = statement;
            else if (identifier is not null && statement.IsReaxValue())
                value = statement;
        }

        if(identifier is null || value is null)
            throw new Exception();
        
        return new AssignmentNode(identifier.Source, value.ToReaxValue());
    }
    
    private IEnumerable<Token> NextStatement() 
    {
        while(true)
        {
            if(_tokens[_position].Type == TokenType.END_STATEMENT)
            {
                _position++;
                break;
            }
            
            yield return _tokens[_position];
            _position++;
        }
    }
}
