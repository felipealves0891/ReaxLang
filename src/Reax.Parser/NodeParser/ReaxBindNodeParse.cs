using System;
using Reax.Core.Debugger;
using Reax.Lexer;
using Reax.Parser.Node;
using Reax.Core.Ast.Statements;
using Reax.Core.Ast;
using Reax.Core.Ast.Expressions;

namespace Reax.Parser.NodeParser;

public class ReaxBindNodeParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.BIND;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        source.Advance(expectedType: TokenType.IDENTIFIER);
        var identifier = source.CurrentToken;
        source.Advance(expectedType: TokenType.COLON);
        source.Advance();
        var dataType = source.CurrentToken;
        source.Advance(expectedType: TokenType.ARROW);
        source.Advance();
        var node = source.NextNode();
        if(node is null)
            throw new InvalidOperationException($"Era esperado o inicio de uma expressão. Posição: {source.CurrentToken.Position}. Linha: {source.CurrentToken.Row}");

        return new BindNode(
            identifier.Source, 
            new AssignmentNode(new VarNode(identifier.Source, dataType.Type.ToDataType(), identifier.Location), node, identifier.Location), 
            dataType.Type.ToDataType(),
            identifier.Location);
    }
}
