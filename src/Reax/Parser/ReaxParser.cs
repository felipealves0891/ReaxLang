using Reax.Lexer;
using Reax.Parser.Helpers;
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

    public bool EndOfTokens => _position >= _tokens.Length;
    public Token BeforeToken => _tokens[_position-1];
    public Token CurrentToken => _tokens[_position];
    public Token NextToken => _tokens[_position+1];

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
        else if(nextToken.Type == TokenType.FUNCTION)
            return FunctionDeclarationParse();
        else if(IsArithmeticOperation())
            return ArithmeticOperationParse();
        else if(nextToken.Type == TokenType.RETURN)
            return ReturnParse();
        
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

        if(identifier is null)
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
        List<Token> parameter = new List<Token>();
        
        foreach (var statement in NextStatement())
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
            
        var values = parameter.Select(x => x.ToReaxValue()).ToArray();
        return new FunctionCallNode(identifier.Source, values);
    }

    private bool IsAssignment() 
    {
        return _tokens[_position].Type == TokenType.IDENTIFIER 
            && _tokens[_position+1].Type == TokenType.ASSIGNMENT;
    }

    private ReaxNode AssignmentParse() 
    {
        Token? identifier = null;
        bool isExpression = false;
        List<Token> expression = new List<Token>();
        ReaxNode? value = null;
        foreach (var statement in NextStatement())
        {
            if(!isExpression && statement.Type == TokenType.IDENTIFIER)
                identifier = statement;
            else if (!isExpression && statement.Type == TokenType.ASSIGNMENT)
                isExpression = true;
            else if (isExpression)
                expression.Add(statement);
        }

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

        if(identifier is null || expression is null)
            throw new Exception();
        
        return new AssignmentNode(identifier.Source, value);
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
        BinaryNode? condition = null;

        if(_tokens[_position].Type == TokenType.WHEN)
        {
            _position++;    
            var left = _tokens[_position++];
            var @perator = _tokens[_position++];
            var right = _tokens[_position++];

            condition = new BinaryNode(
                left.ToReaxValue(), 
                @perator.ToLogicOperator(), 
                right.ToReaxValue());
        }

        if(_tokens[_position].Type == TokenType.START_BLOCK)
            return new ObservableNode(variable, NextBlock(), condition);
        else if(_tokens[_position].Type == TokenType.ARROW)
            return  new ObservableNode(variable, new ContextNode([ArrowParse()]), condition);
        else
            throw new InvalidOperationException($"Token invalido '{_tokens[_position].Type}' na posição: {_position}");
    }

    private bool IsArithmeticOperation() 
    {
        return CurrentToken.CanCalculated() && NextToken.IsArithmeticOperator();
    }

    private ReaxNode ArithmeticOperationParse() 
    {
        var statement = NextStatement();
        var helper = new CalculationHelper(statement);
        var node = helper.ParseExpression();
        
        if(node is null)
            throw new InvalidOperationException("Valores faltando para a operação");

        return node;
    }

    private ReaxNode ArrowParse() 
    {
        _position++;
        return NextNode() ?? throw new InvalidOperationException();
    }

    private ReaxNode FunctionDeclarationParse() 
    {
        _position++;
        var identifier = CurrentToken.ToReaxValue();
        _position++;
        var parameters = GetParameters().ToArray();
        var block = NextBlock();
        return new FunctionNode(identifier, block, parameters);
    }

    private IEnumerable<ReaxNode> GetParameters() 
    {
        var parameters = new List<ReaxNode>();
        if(CurrentToken.Type != TokenType.START_PARAMETER)
            return parameters;

        _position++;
        while(CurrentToken.Type != TokenType.END_PARAMETER) 
        {
            if(CurrentToken.Type == TokenType.IDENTIFIER)
                parameters.Add(CurrentToken.ToReaxValue());

            _position++;
        }
        _position++;
        return parameters;
    }

    private ReaxNode ReturnParse() 
    {
        _position++;
        var statement = NextStatement().ToArray();
        if(statement.Length == 1)
            return statement[0].ToReaxValue();
        
        var parser = new ReaxParser(statement);
        var context = parser.Parse();
        return new ContextNode(context.ToArray());
    }

    private IEnumerable<Token> NextStatement() 
    {
        while(true)
        {
            if(EndOfTokens)
                break;

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
        while(_tokens[_position].Type != TokenType.END_BLOCK)
        {
            var node = NextNode();
            if(node is null)
                throw new InvalidOperationException("Era esperado o fim do bloco!");

            block.Add(node);
        }

        _position++;
        return new ContextNode(block.ToArray());
    }
}
