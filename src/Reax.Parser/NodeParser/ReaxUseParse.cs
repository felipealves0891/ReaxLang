using System;
using Reax.Core.Ast;
using Reax.Core.Ast.Expressions;
using Reax.Core.Locations;
using Reax.Core.Types;
using Reax.Lexer;
using Reax.Parser.Extensions;
using Reax.Parser.Helper;

namespace Reax.Parser.NodeParser;

public class ReaxUseInstanceParse : INodeParser
{
    private static TokenType[] POSSIBILITIES_AFTER_USE_OR_IN = [TokenType.NATIVE_IDENTIFIER, TokenType.IDENTIFIER, TokenType.NUMBER_LITERAL, TokenType.STRING_LITERAL];
    private static TokenType[] POSSIBILITIES_AFTER_EXPRESSION = [TokenType.OPEN_PARENTHESIS, TokenType.IN, TokenType.OF, TokenType.AS];

    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.USE;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        return ParsePrimary(source);
    }

    private ReaxNode ParsePrimary(ITokenSource source)
    {
        if (source.CurrentToken.Type == TokenType.USE)
            return ParseNativeCall(source);
        else if (source.CurrentToken.Type == TokenType.IN)
            return ParseNativeCall(source);
        else if (source.CurrentToken.IsReaxValue())
            return source.CurrentToken.ToReaxValue();
        else
            throw new Exception("Expressão inválida");
    }

    private ReaxNode ParseNativeCall(ITokenSource source)
    {
        var expression = ParseExpression(source);
        if(source.CurrentToken.Type != TokenType.AS)
            source.Advance(TokenType.AS);

        var type = DefineAs(source);
        source.Advance(TokenType.SEMICOLON);
        source.Advance();
        return new NativeCallNode(expression, type, expression.Location);
    }

    private ReaxNode ParseExpression(ITokenSource source)
    {
        source.Advance(POSSIBILITIES_AFTER_USE_OR_IN);
        var (identifier, arguments) = GetIdentifierAndParameters(source);

        if (source.CurrentToken.Type == TokenType.IN)
            return ParseIn(source, identifier, arguments);
        else if (source.CurrentToken.Type == TokenType.OF)
            return ParseOf(source, identifier, arguments);
        else if (identifier.IsReaxValue())
            return identifier.ToReaxValue();
        else
            throw new InvalidOperationException("Expressão inválida");
    }
    
    private ReaxNode ParseIn(ITokenSource source, Token identifier, IEnumerable<ReaxNode> arguments)
    {
        if (identifier.IsReaxValue())
            return identifier.ToReaxValue();
    
        var target = ParseExpression(source);
        return new UseInstanceNode(
                identifier.Source,
                arguments.ToArray(),
                target,
                new SourceLocation(
                    identifier.File,
                    identifier.Location.Start,
                    target.Location.End));
    }

    private ExpressionNode ParseOf(ITokenSource source, Token identifier, IEnumerable<ReaxNode> arguments)
    { 
        source.Advance(TokenType.NATIVE_IDENTIFIER);
        var typeName = source.CurrentToken;
        return new UseStaticNode(
            identifier.Source,
            typeName.Source,
            arguments.ToArray(),
            new SourceLocation(
                identifier.File,
                identifier.Location.Start,
                typeName.Location.End));
    }

    private (Token, IEnumerable<ReaxNode>) GetIdentifierAndParameters(ITokenSource source)
    {
        var identifier = source.CurrentToken;
        var arguments = Enumerable.Empty<ReaxNode>();
        source.Advance(POSSIBILITIES_AFTER_EXPRESSION);

        if (source.CurrentToken.Type == TokenType.OPEN_PARENTHESIS)
            arguments = ParameterHelper.GetCallParameters(source, false);

        return (identifier, arguments);
    }

    private DataType DefineAs(ITokenSource source)
    {
        var dataType = DataType.NONE;
        if (source.CurrentToken.Type == TokenType.AS)
        {
            source.Advance(Token.DataTypes);
            dataType = source.CurrentToken.Type.ToDataType();
            if (source.NextToken.Type == TokenType.OPEN_BRACKET)
            {
                dataType = dataType | DataType.ARRAY;
                source.Advance(TokenType.OPEN_BRACKET);
                source.Advance(TokenType.CLOSE_BRACKET);
            }        
        }

        return dataType;
    }
}
