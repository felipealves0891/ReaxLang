using System;
using Reax.Core.Ast;
using Reax.Core.Ast.Expressions;
using Reax.Core.Locations;
using Reax.Lexer;
using Reax.Parser.Extensions;
using Reax.Parser.Helper;

namespace Reax.Parser.NodeParser;

public class ReaxUseInstanceParse : INodeParser
{
    private static TokenType[] POSSIBILITIES_AFTER_USE_OR_IN = [TokenType.NATIVE_IDENTIFIER, TokenType.IDENTIFIER, TokenType.NUMBER_LITERAL, TokenType.STRING_LITERAL];
    private static TokenType[] POSSIBILITIES_AFTER_EXPRESSION = [TokenType.OPEN_PARENTHESIS, TokenType.IN, TokenType.OF];

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
            return ParseExpression(source);
        else if (source.CurrentToken.Type == TokenType.IN)
            return ParseExpression(source);
        else if (source.CurrentToken.IsReaxValue())
            return source.CurrentToken.ToReaxValue();
        else
            throw new Exception("Expressão inválida");
    }

    private ReaxNode ParseExpression(ITokenSource source)
    {
        source.Advance(POSSIBILITIES_AFTER_USE_OR_IN);
        var identifier = source.CurrentToken;
        if(identifier.IsReaxValue())
            return source.CurrentToken.ToReaxValue();

        var arguments = Enumerable.Empty<ReaxNode>();
        source.Advance(POSSIBILITIES_AFTER_EXPRESSION);
        if (source.CurrentToken.Type == TokenType.OPEN_PARENTHESIS)
            arguments = ParameterHelper.GetCallParameters(source, false);

        if (source.CurrentToken.Type == TokenType.IN)
        {
            var target = ParsePrimary(source);
            source.Advance(TokenType.AS);
            source.Advance(Token.DataTypes);
            var dataType = source.CurrentToken;
            source.Advance(TokenType.SEMICOLON);
            source.Advance();
            return new UseInstanceNode(
                identifier.Source,
                arguments.ToArray(),
                target,
                dataType.Type.ToDataType(),
                new SourceLocation(
                    identifier.File,
                    identifier.Location.Start,
                    target.Location.End));
        }

        if (source.CurrentToken.Type == TokenType.OF)
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

        throw new Exception("Expressão inválida");
    }

}
