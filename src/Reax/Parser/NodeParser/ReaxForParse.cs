using System;
using Reax.Debugger;
using Reax.Lexer;
using Reax.Parser.Node;
using Reax.Parser.Node.Operations;

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
        source.Advance();
        var dataType = source.CurrentToken;
        source.Advance();
        if(source.CurrentToken.Type != TokenType.ASSIGNMENT)
            throw new InvalidOperationException("Era esperado uma atribuição!");
        source.Advance();
        var initialValue = source.CurrentToken;
        source.Advance();
        var declaration = new DeclarationNode(
            identifierControl.Source, 
            false, 
            false, 
            dataType.Type.ToDataType(),
            initialValue.ToReaxValue(), 
            identifierControl.Location);
            
        if(source.CurrentToken.Type != TokenType.TO)
            throw new InvalidOperationException("Era esperado uma expresão 'TO'!");
        source.Advance();

        var limitValue = source.CurrentToken;
        var condition = new BinaryNode(
            identifierControl.ToReaxValue(), 
            new ComparisonNode("<", identifierControl.Location), 
            limitValue.ToReaxValue(),
            identifierControl.Location);
        
        source.Advance();

        var block = (ContextNode)source.NextBlock();

        var node = new ForNode(declaration, condition, block, declaration.Location);
        Logger.LogParse(node.ToString());
        return node;
    }
}
