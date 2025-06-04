using System;
using Reax.Lexer;
using Reax.Parser.Extensions;
using Reax.Parser.Node;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Literals;
using Reax.Core.Ast.Operations;
using Reax.Core.Ast;
using Reax.Core.Types;
using Reax.Core.Ast.Objects.Structs;
using Reax.Core.Locations;

namespace Reax.Parser.Helper;

public class ExpressionHelper
{
    private readonly Token[] _tokens;
    private int _pos;

    private ExpressionHelper(Token[] tokens)
    {
        _tokens = tokens;
    }

    public static ReaxNode Parser(Token[] tokens)
    {
        var helper = new ExpressionHelper(tokens);
        return helper.ParseExpression();
    }

    public static ReaxNode Parser(IEnumerable<Token> tokens)
    {
        var helper = new ExpressionHelper(tokens.ToArray());
        return helper.ParseExpression();
    }

    public static BinaryNode ParserBinary(Token[] tokens)
    {
        var helper = new ExpressionHelper(tokens);
        var result = helper.ParseExpression();
        if (result is BinaryNode binary)
            return binary;
        else
            return new BinaryNode(result, new EqualityNode("==", result.Location), new BooleanNode("true", result.Location), result.Location);
    }

    private Token Peek() => _pos < _tokens.Length ? _tokens[_pos] : new Token(TokenType.EOF, (byte)' ', "", -1, -1);
    private Token Consume() => _pos + 1 <= _tokens.Length ? _tokens[_pos++] : new Token(TokenType.EOF, (byte)' ', "", -1, -1);

    private ReaxNode ParseExpression()
    {
        var value = ParsePrefered();

        while (Peek().Type == TokenType.COMPARISON || Peek().Type == TokenType.EQUALITY || Peek().Type == TokenType.TERM)
        {
            var op = Consume();
            var right = ParsePrefered();

            if (op.Type == TokenType.TERM)
                value = new CalculateNode(value, op.ToArithmeticOperator(), right, op.Location);
            else
                value = new BinaryNode(value, op.ToLogicOperator(), right, op.Location);
        }

        return value;
    }

    private ReaxNode ParsePrefered()
    {
        var value = ParseValue();
        while (Peek().Type == TokenType.FACTOR)
        {
            var op = Consume();
            var right = ParseValue();
            value = new CalculateNode(value, op.ToArithmeticOperator(), right, op.Location);
        }
        return value;
    }

    private ReaxNode ParseValue()
    {
        var token = Peek();
        if (token.IsReaxValue())
        {
            return Parse(token);
        }
        else if (token.ReadOnlySource.SequenceEqual([(byte)'-']))
        {
            var unary = Consume();
            var value = Consume().AppendAtBeginning(unary);
            return value.ToReaxValue();
        }
        else if (token.Type == TokenType.OPEN_PARENTHESIS)
        {
            Consume();
            var node = ParseExpression();
            var peek = Peek();
            if (peek.Type != TokenType.CLOSE_PARENTHESIS)
                throw new InvalidOperationException("Esperado ')'");
            Consume();
            return node;
        }
        else if (token.Type == TokenType.USE)
        {
            return NativeCallHelper.Parser(_tokens[_pos..]);
        }
        else if (token.Type == TokenType.OPEN_BRACKET)
        {
            return ArrayHelper.Parse(_tokens[_pos..]);
        }
        else if (token.Type == TokenType.INVOKE)
        {
            var keyword = Consume();
            var identifier = Consume();
            return new InvokeNode(identifier.Source, 
                new SourceLocation(
                    keyword.File,
                    keyword.Location.Start,
                    identifier.Location.End));
        }
        else
        {
            throw new Exception($"Token inesperado: {token}");
        }
    }

    private ReaxNode Parse(Token token)
    {
        Consume();

        var peek = Peek();
        if (peek.Type == TokenType.OPEN_BRACKET)
        {
            Consume();
            var variable = new VarNode(token.Source, DataType.NONE, token.Location);
            var expression = ParseExpression();
            return new ArrayAccessNode(variable, expression, expression.Location);
        }
        else if (peek.Type == TokenType.ACCESS)
        {
            Consume();
            var functionCall = (FunctionCallNode)Parse(Peek());
            return new ExternalFunctionCallNode(token.Source, functionCall, token.Location);
        }
        else if (peek.Type == TokenType.ARROW)
        {
            return ParseStructureFieldAccess(token);
        }
        else if (peek.Type == TokenType.OPEN_PARENTHESIS)
        {
            Consume();
            var identifier = token;
            var listParameters = new List<ReaxNode>();

            while (Peek().Type != TokenType.CLOSE_PARENTHESIS && Peek().Type != TokenType.EOF)
            {
                if (Peek().Type != TokenType.COMMA)
                    listParameters.Add(ParseValue());

                Consume();
            }

            Consume();
            return new FunctionCallNode(identifier.Source, listParameters.ToArray(), identifier.Location);
        }
        else
        {
            return token.ToReaxValue();
        }
    }

    private ReaxNode ParseStructureFieldAccess(Token instance)
    {
        Consume();
        var property = Consume();

        var structFieldAccessNode
            = new StructFieldAccessNode(
                instance.Source,
                property.Source,
                new SourceLocation(
                    instance.File,
                    instance.Location.Start,
                    property.Location.End));

        if (Peek().Type == TokenType.OPEN_BRACKET)
        {
            Consume();
            var array = ParseExpression();
            return new ArrayAccessNode(structFieldAccessNode, array, structFieldAccessNode.Location);
        }

        Consume();
        return structFieldAccessNode;
    }

}
