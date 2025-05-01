using System;
using Reax.Debugger;
using Reax.Lexer;
using Reax.Parser.Node;

namespace Reax.Parser.NodeParser;

public class ReaxScriptDeclarationParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.SCRIPT && next.Type == TokenType.IDENTIFIER;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        var location = source.CurrentToken.Location;

        source.Advance();
        var identifier = source.CurrentToken;
        source.Advance();
        source.Advance();

        return new ScriptDeclarationNode(identifier.Source, location);
    }
}
