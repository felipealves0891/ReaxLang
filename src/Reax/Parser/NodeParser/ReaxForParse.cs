using System;
using Reax.Core.Types;
using Reax.Debugger;
using Reax.Lexer;
using Reax.Parser.Node;
using Reax.Parser.Node.Expressions;
using Reax.Parser.Node.Operations;
using Reax.Parser.Node.Statements;

namespace Reax.Parser.NodeParser;

public class ReaxForParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.FOR;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        source.Advance(TokenType.IDENTIFIER);
        var identifierControl = source.CurrentToken;
        source.Advance(TokenType.TYPING);
        source.Advance([TokenType.FLOAT_TYPE, TokenType.LONG_TYPE, TokenType.INT_TYPE]);
        var dataType = source.CurrentToken;
        source.Advance(TokenType.ASSIGNMENT);
        source.Advance(TokenType.NUMBER_LITERAL);
        var initialValue = source.CurrentToken;
        source.Advance(TokenType.TO);

        var declaration = new DeclarationNode(
            identifierControl.Source, 
            false, 
            false, 
            dataType.Type.ToDataType(),
            new AssignmentNode(new VarNode(identifierControl.Source, DataType.NUMBER, identifierControl.Location), initialValue.ToReaxValue(), initialValue.Location), 
            identifierControl.Location);
            
        source.Advance(TokenType.NUMBER_LITERAL);

        var limitValue = source.CurrentToken;
        var condition = new BinaryNode(
            identifierControl.ToReaxValue(), 
            new ComparisonNode("<", identifierControl.Location), 
            limitValue.ToReaxValue(),
            identifierControl.Location);
        
        source.Advance(TokenType.START_BLOCK);
        var block = (ContextNode)source.NextBlock();
        return new ForNode(declaration, condition, block, declaration.Location);
    }
}
