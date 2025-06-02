using System;
using Reax.Core.Ast;
using Reax.Core.Ast.Expressions;
using Reax.Core.Locations;
using Reax.Core.Types;
using Reax.Lexer;
using Reax.Parser.Extensions;

namespace Reax.Parser.Helper;

public class NativeCallHelper
{
    private static TokenType[] POSSIBILITIES_AFTER_USE_OR_IN = [TokenType.NATIVE_IDENTIFIER, TokenType.IDENTIFIER, TokenType.NUMBER_LITERAL, TokenType.STRING_LITERAL];
    private static TokenType[] POSSIBILITIES_AFTER_EXPRESSION = [TokenType.OPEN_PARENTHESIS, TokenType.IN, TokenType.OF, TokenType.AS];

    private readonly Token[] _tokens;
    private int _pos;

    private NativeCallHelper(Token[] tokens)
    {
        _tokens = tokens;
        _pos = 0;
    }

    private Token Peek() => _pos < _tokens.Length ? _tokens[_pos] : new Token(TokenType.EOF, (byte)' ', "", -1, -1);
    private Token Consume() => _pos + 1 <= _tokens.Length ? _tokens[_pos++] : new Token(TokenType.EOF, (byte)' ', "", -1, -1);

    private ReaxNode ParsePrimary()
    {
        if (Peek().Type == TokenType.USE)
            return ParseNativeCall();
        else if (Peek().Type == TokenType.IN)
            return ParseNativeCall();
        else if (Peek().IsReaxValue())
            return Peek().ToReaxValue();
        else
            throw new Exception("Expressão inválida");
    }

    private ReaxNode ParseNativeCall()
    {
        var expression = ParseExpression();
        if (Peek().Type != TokenType.AS)
            Advance(TokenType.AS);

        var type = DefineAs();
        Consume();
        return new NativeCallNode(expression, type, expression.Location);
    }

    private ReaxNode ParseExpression()
    {
        Advance(POSSIBILITIES_AFTER_USE_OR_IN);
        var (identifier, arguments) = GetIdentifierAndParameters();

        if (Peek().Type == TokenType.IN)
            return ParseIn(identifier, arguments);
        else if (Peek().Type == TokenType.OF)
            return ParseOf(identifier, arguments);
        else if (identifier.IsReaxValue())
            return identifier.ToReaxValue();
        else
            throw new InvalidOperationException("Expressão inválida");
    }

    private ReaxNode ParseIn(Token identifier, IEnumerable<ReaxNode> arguments)
    {
        if (identifier.IsReaxValue())
            return identifier.ToReaxValue();

        var target = ParseExpression();
        return new UseInstanceNode(
                identifier.Source,
                arguments.ToArray(),
                target,
                new SourceLocation(
                    identifier.File,
                    identifier.Location.Start,
                    target.Location.End));
    }

    private ExpressionNode ParseOf(Token identifier, IEnumerable<ReaxNode> arguments)
    {
        Advance(TokenType.NATIVE_IDENTIFIER);
        var typeName = Consume();
        return new UseStaticNode(
            identifier.Source,
            typeName.Source,
            arguments.ToArray(),
            new SourceLocation(
                identifier.File,
                identifier.Location.Start,
                typeName.Location.End));
    }

    private (Token, IEnumerable<ReaxNode>) GetIdentifierAndParameters()
    {
        var identifier = Consume();
        var arguments = Enumerable.Empty<ReaxNode>();

        if (!Match(POSSIBILITIES_AFTER_EXPRESSION))
            throw new InvalidOperationException("Expressão inválida");

        if (Peek().Type == TokenType.OPEN_PARENTHESIS)
            arguments = GetCallParameters();

        return (identifier, arguments);
    }

    private IEnumerable<ReaxNode> GetCallParameters()
    {
        var parameters = new List<ReaxNode>();
        if (Peek().Type != TokenType.OPEN_PARENTHESIS)
            return parameters;

        Consume();
        TokenType[] allowType = [TokenType.IDENTIFIER, TokenType.TRUE, TokenType.FALSE, TokenType.NUMBER_LITERAL, TokenType.STRING_LITERAL];
        var current = Peek();
        while (current.Type != TokenType.CLOSE_PARENTHESIS)
        {
            if (allowType.Contains(Peek().Type))
            {
                var tokens = new List<Token>();

                while (Peek().Type != TokenType.COMMA && Peek().Type != TokenType.CLOSE_PARENTHESIS)
                {
                    tokens.Add(Peek());
                    Consume();
                }

                parameters.Add(ExpressionHelper.Parser(tokens));
            }

            if (Peek().Type == TokenType.CLOSE_PARENTHESIS && Peek().Type != TokenType.CLOSE_PARENTHESIS) break;
            current = Consume();
        }

        return parameters;
    }

    private DataType DefineAs()
    {
        var dataType = DataType.NONE;
        if (Peek().Type == TokenType.AS)
        {
            Consume();
            if (!Match(Token.DataTypes))
                throw new Exception("Expressão inválida");

            dataType = Consume().Type.ToDataType();
            var next = Consume();
            if (next.Type == TokenType.OPEN_BRACKET)
            {
                dataType = dataType | DataType.ARRAY;
                if (Consume().Type != TokenType.CLOSE_BRACKET)
                    throw new Exception("Expressão inválida");
            }
        }
        return dataType;
    }

    private void Advance(params TokenType[] types)
    {
        Consume();
        if (!Match(types))
            throw new InvalidOperationException("Expressão inválida");
    }

    private bool Match(params TokenType[] types)
    {
        return types.Contains(Peek().Type);
    }

    public static ReaxNode Parser(Token[] tokens)
    {
        var helper = new NativeCallHelper(tokens);
        return helper.ParsePrimary();
    }
}
