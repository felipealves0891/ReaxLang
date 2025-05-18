using System;
using Reax.Core.Ast;
using Reax.Core.Ast.Objects.Structs;
using Reax.Core.Locations;
using Reax.Lexer;

namespace Reax.Parser.NodeParser;

public class ReaxStructFieldAccessParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return before.Type == TokenType.ASSIGNMENT
            && current.Type == TokenType.IDENTIFIER
            && next.Type == TokenType.ARROW;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        var instance = source.CurrentToken;
        source.Advance(TokenType.ARROW);
        source.Advance(TokenType.IDENTIFIER);
        var property = source.CurrentToken;
        source.Advance(TokenType.SEMICOLON);
        source.Advance();
        return new StructFieldAccessNode(
            instance.Source,
            property.Source,
            new SourceLocation(
                instance.File,
                instance.Location.Start,
                property.Location.End));

    }
}
