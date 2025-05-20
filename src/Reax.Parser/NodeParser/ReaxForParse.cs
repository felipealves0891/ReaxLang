using System;
using Reax.Core.Types;
using Reax.Core.Debugger;
using Reax.Lexer;
using Reax.Parser.Node;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Operations;
using Reax.Core.Ast.Statements;
using Reax.Parser.Extensions;
using Reax.Core.Ast;
using Reax.Parser.Helper;

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
        source.Advance(TokenType.COLON);
        source.Advance(Token.DataTypes);
        var dataType = source.CurrentToken;

        if (source.NextToken.Type == TokenType.IN)
            return ParseIn(source, identifierControl, dataType);
        else
            return ParseTo(source, identifierControl, dataType);
    }

    private ReaxNode ParseTo(ITokenSource source, Token identifierControl, Token dataType)
    {
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
            null,
            identifierControl.Location);

        source.Advance(TokenType.NUMBER_LITERAL);

        var limitValue = source.CurrentToken;
        var condition = new BinaryNode(
            identifierControl.ToReaxValue(),
            new ComparisonNode("<", identifierControl.Location),
            limitValue.ToReaxValue(),
            identifierControl.Location);

        source.Advance(TokenType.OPEN_BRACE);
        var block = (ContextNode)source.NextBlock();
        return new ForNode(declaration, condition, block, declaration.Location);
    }

    private ReaxNode ParseIn(ITokenSource source, Token identifierControl, Token dataType)
    {
        source.Advance(TokenType.IN);
        source.Advance([TokenType.IDENTIFIER, TokenType.OPEN_BRACKET]);
        var array = GetArray(source);
        var declaration = new DeclarationNode(
            identifierControl.Source,
            false,
            false,
            dataType.Type.ToDataType(),
            null,
            null,
            identifierControl.Location);

        if(source.NextToken.Type == TokenType.OPEN_BRACE)
            source.Advance(TokenType.OPEN_BRACE);
            
        var block = (ContextNode)source.NextBlock();
        return new ForInNode(declaration, array, block, declaration.Location);
    }

    private ReaxNode GetArray(ITokenSource source)
    {
        var start = source.CurrentToken;
        if (source.CurrentToken.Type == TokenType.IDENTIFIER && source.NextToken.Type != TokenType.ARROW)
            return new VarNode(source.CurrentToken.Source, DataType.NONE, source.CurrentToken.Location);

        if (source.CurrentToken.Type == TokenType.IDENTIFIER && source.NextToken.Type == TokenType.ARROW)
            return ExpressionHelper.Parser(source.NextExpression());

        var arrayParse = new ReaxArrayParse();
        return arrayParse.Parse(source)
            ?? throw new InvalidOperationException($"{start.Source} - Era esperado um array no for in!");
    }
}
