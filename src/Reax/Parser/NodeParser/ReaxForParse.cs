using System;
using Reax.Lexer;
using Reax.Parser.Node;

namespace Reax.Parser.NodeParser;

public class ReaxForParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.FOR;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        source.Advance();
        var identifierControl = source.CurrentToken;
        source.Advance();
        if(source.CurrentToken.Type != TokenType.ASSIGNMENT)
            throw new InvalidOperationException("Era esperado uma atribuição!");
        source.Advance();
        var initialValue = source.CurrentToken;
        source.Advance();
        var declaration = new DeclarationNode(identifierControl.Source, initialValue.ToReaxValue());
        if(source.CurrentToken.Type != TokenType.TO)
            throw new InvalidOperationException("Era esperado uma expresão 'TO'!");
        source.Advance();

        var limitValue = source.CurrentToken;
        var condition = new BinaryNode(
            identifierControl.ToReaxValue(), 
            new ComparisonNode("<"), 
            limitValue.ToReaxValue());
        
        source.Advance();

        var block = source.NextBlock();
        return new ForNode(declaration, condition, block);
    }
}
