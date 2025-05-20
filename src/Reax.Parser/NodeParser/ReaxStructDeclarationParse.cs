using System;
using Reax.Core.Ast;
using Reax.Core.Ast.Statements;
using Reax.Core.Locations;
using Reax.Lexer;

namespace Reax.Parser.NodeParser;

public class ReaxStructDeclarationParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.STRUCT
            && next.Type == TokenType.IDENTIFIER;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        var start = source.CurrentToken;
        source.Advance(TokenType.IDENTIFIER);
        var identifier = source.CurrentToken;
        source.Advance(TokenType.OPEN_BRACE);
        var properties = new List<StructFieldNode>();
        
        do
        {
            source.Advance(TokenType.IDENTIFIER);
            var propertyName = source.CurrentToken;
            source.Advance(TokenType.COLON);
            source.Advance(Token.DataTypes);
            var propertyType = source.CurrentToken.Type.ToDataType();
            if (source.NextToken.Type == TokenType.OPEN_BRACKET)
            {
                source.Advance(TokenType.OPEN_BRACKET);
                propertyType = propertyType | Core.Types.DataType.ARRAY;
                source.Advance(TokenType.CLOSE_BRACKET);
            }

            properties.Add(new StructFieldNode(
                propertyName.Source,
                propertyType,
                new SourceLocation(
                    propertyName.File,
                    propertyName.Location.Start,
                    propertyName.Location.End)));

            source.Advance([TokenType.COMMA, TokenType.CLOSE_BRACE]);
        }
        while (source.CurrentToken.Type != TokenType.CLOSE_BRACE);

        var end = source.CurrentToken;
        source.Advance();
        return new StructDeclarationNode(
            identifier.Source,
            properties,
            new SourceLocation(start.File, start.Location.Start, end.Location.End));
    }
}
