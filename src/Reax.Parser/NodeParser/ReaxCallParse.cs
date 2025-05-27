using System;
using Reax.Core.Ast;
using Reax.Core.Ast.Statements;
using Reax.Core.Locations;
using Reax.Lexer;
using Reax.Parser.Helper;

namespace Reax.Parser.NodeParser;

public class ReaxCallParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.CALL;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        var call = source.CurrentToken;
        source.Advance(TokenType.NATIVE_IDENTIFIER);

        var methodName = source.CurrentToken;
        source.Advance(TokenType.OPEN_PARENTHESIS);

        var arguments = ParameterHelper.GetCallParameters(source, false);
        source.Advance(TokenType.NATIVE_IDENTIFIER);

        var typeName = source.CurrentToken;
        source.Advance(TokenType.SEMICOLON);
        source.Advance();

        return new CallStaticNode(
            typeName.Source,
            methodName.Source,
            arguments.ToArray(),
            new SourceLocation(
                call.File,
                call.Location.Start,
                typeName.Location.End));
    }
}
