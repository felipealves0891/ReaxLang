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
        else if(nextToken.Type == TokenType.IF)
            return IfParse();
        else if(nextToken.Type == TokenType.ON)
            return ObservableParse();

        throw new Exception();
    }

    private ReaxNode DeclarationParse() 
    {
        Token? identifier = null;
        Token? value = null;

        foreach (var statement in NextStatement())
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
        bool startParameter = false;

        Token? identifier = null;
        Token? parameter = null;
        
        foreach (var statement in NextStatement())
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

    private ReaxNode IfParse()
    {
        _position++;
        var left = _tokens[_position++];
        var @perator = _tokens[_position++];
        var right = _tokens[_position++];
        var @true = NextBlock();
        ReaxNode? @else = null;

        if(_tokens[_position].Type == TokenType.ELSE)
        {
            _position++;
            @else = NextBlock();
        }

        var condition = new BinaryNode(
            left.ToReaxValue(), 
            @perator.ToLogicOperator(), 
            right.ToReaxValue());

        return new IfNode(condition, @true, @else);
    }
    
    private ReaxNode ObservableParse() 
    {
        _position++;
        var variable = new VarNode(_tokens[_position++].Source);

        if(_tokens[_position].Type == TokenType.START_BLOCK)
            return new ObservableNode(variable, NextBlock());
        else if(_tokens[_position].Type == TokenType.ARROW)
            return  new ObservableNode(variable, new ContextNode([ArrowParse()]));
        else
            throw new InvalidOperationException($"Token invalido '{_tokens[_position].Type}' na posição: {_position}");
    }

    private ReaxNode ArrowParse() 
    {
        _position++;
        return NextNode() ?? throw new InvalidOperationException();
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

    private ReaxNode NextBlock()
    {
        var block = new List<ReaxNode>();
        if(_tokens[_position].Type != TokenType.START_BLOCK)
            return new ContextNode(block.ToArray());

        _position++;
        while(true)
        {
            if(_tokens[_position].Type == TokenType.END_BLOCK)
                break;

            var node = NextNode();
            if(node is null)
                throw new InvalidOperationException("Era esperado o fim do bloco!");

            block.Add(node);
        }

        _position++;
        return new ContextNode(block.ToArray());
    }
}
