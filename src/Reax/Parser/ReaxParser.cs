using System.Collections.ObjectModel;
using Reax.Lexer;
using Reax.Parser.Helpers;
using Reax.Parser.Node;

namespace Reax.Parser;

public class ReaxParser
{
    private readonly IList<string> _imports;
    private readonly Token[] _tokens;
    private int _position;

    public ReaxParser(IEnumerable<Token> tokens)
    {
        _tokens = tokens.ToArray();
        _imports = new List<string>();
        _position = 0;
    }

    public IReadOnlyCollection<string> Imports => new ReadOnlyCollection<string>(_imports);

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
        if(nextToken.Type == TokenType.IMPORT)
            return ImportScripts();
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
        else if(nextToken.Type == TokenType.FOR)
            return ForParse();
        else if(nextToken.Type == TokenType.WHILE)
            return WhileParse();
        
        throw new Exception($"Token invalido na linha {CurrentToken.Row}: {CurrentToken}");
    }

    private ReaxNode? ImportScripts() 
    {
        Advance();
        var file = CurrentToken.Source;
        _imports.Add(file);
        Advance();
        if(CurrentToken.Type == TokenType.END_STATEMENT)
            throw new InvalidOperationException($"Era esperado o fim da expressão na linha {CurrentToken.Row}!");

        Advance();
        return NextNode();
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

        return new AssignmentNode(identifier.Source, value);
    }

    private ReaxNode IfParse()
    {
        _position++;
        var statement = NextStatement();
        var comparisonHelper = new ComparisonHelper(statement);
        var condition = comparisonHelper.Parse();
        var @true = NextBlock();
        ReaxNode? @else = null;

        if(_tokens[_position].Type == TokenType.ELSE)
        {
            _position++;
            @else = NextBlock();
        }


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
            var statement = NextStatement();
            var comparisonHelper = new ComparisonHelper(statement);
            condition = comparisonHelper.Parse();
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

    private ReaxNode ForParse() 
    {
        Advance();
        var identifierControl = CurrentToken;
        Advance();
        if(CurrentToken.Type != TokenType.ASSIGNMENT)
            throw new InvalidOperationException("Era esperado uma atribuição!");
        Advance();
        var initialValue = CurrentToken;
        Advance();
        var declaration = new DeclarationNode(identifierControl.Source, initialValue.ToReaxValue());
        if(CurrentToken.Type != TokenType.TO)
            throw new InvalidOperationException("Era esperado uma expresão 'TO'!");
        Advance();

        var limitValue = CurrentToken;
        var condition = new BinaryNode(
            identifierControl.ToReaxValue(), 
            new ComparisonNode("<"), 
            limitValue.ToReaxValue());
        
        Advance();

        var block = NextBlock();
        return new ForNode(declaration, condition, block);
    }

    private ReaxNode WhileParse() 
    {
        Advance();
        var statement = NextStatement();
        var helper = new ComparisonHelper(statement);
        var condition = helper.Parse();

        var block = NextBlock();
        return new WhileNode(condition, block);
    }

    private IEnumerable<Token> NextStatement() 
    {
        while(true)
        {
            if(EndOfTokens)
                break;
            
            if(CurrentToken.Type == TokenType.START_BLOCK)                
                break;
        
            if(CurrentToken.Type == TokenType.END_STATEMENT) 
            {
                Advance();
                break;
            }
            
            yield return CurrentToken;
            Advance();
        }
    }

    private ReaxNode NextBlock()
    {
        var block = new List<ReaxNode>();
        if(CurrentToken.Type != TokenType.START_BLOCK)
            return new ContextNode(block.ToArray());

        Advance();
        while(CurrentToken.Type != TokenType.END_BLOCK)
        {
            var node = NextNode();
            if(node is null)
                throw new InvalidOperationException("Era esperado o fim do bloco!");

            block.Add(node);
        }

        Advance();
        return new ContextNode(block.ToArray());
    }

    private void Advance() 
    {
        if(_position + 1 > _tokens.Length)
            throw new InvalidOperationException("Não é possivel avançar após o fim dos tokens");

        _position++;
    }
}
