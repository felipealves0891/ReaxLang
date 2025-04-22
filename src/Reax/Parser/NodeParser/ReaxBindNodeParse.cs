using System;
using Reax.Debugger;
using Reax.Lexer;
using Reax.Parser.Node;

namespace Reax.Parser.NodeParser;

public class ReaxBindNodeParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.BIND;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        source.Advance();
        var identifier = source.CurrentToken;
        source.Advance();
        source.Advance();
        var dataType = source.CurrentToken;
        source.Advance();
        
        if(source.CurrentToken.Type != TokenType.ARROW)
            throw new InvalidOperationException($"Token invalido na linha {source.CurrentToken.Row}, era esperado o inicio de uma expressão. Posição: {source.CurrentToken.Position}.");
            
        source.Advance();
        var node = source.NextNode();
        if(node is null)
            throw new InvalidOperationException($"Era esperado o inicio de uma expressão. Posição: {source.CurrentToken.Position}. Linha: {source.CurrentToken.Row}");

        var bind = new BindNode(
            new IdentifierNode(identifier.Source.ToString(), identifier.Location), 
            new ContextNode([node], identifier.Location), 
            new DataTypeNode(dataType.Source, dataType.Location), 
            identifier.Location);
            
        Logger.LogParse(bind.ToString());
        return bind;
    }
}
