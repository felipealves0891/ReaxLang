using System;
using Reax.Core.Debugger;
using Reax.Lexer;
using Reax.Parser.Extensions;
using Reax.Parser.Node;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast;
using Reax.Parser.Helper;

namespace Reax.Parser.NodeParser;

public class ReaxExternalFunctionCallParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.IDENTIFIER && next.Type == TokenType.ACCESS;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        var scriptName = source.CurrentToken;
        source.Advance(TokenType.ACCESS);
        source.Advance(TokenType.IDENTIFIER);
        var identifier = source.CurrentToken;
        source.Advance(TokenType.OPEN_PARENTHESIS);
        var parameters = ParameterHelper.GetCallParameters(source);
        source.Advance();
        return new ExternalFunctionCallNode(
            scriptName.Source, 
            new FunctionCallNode(identifier.Source, parameters.ToArray(), identifier.Location),
            identifier.Location);
    }
}
