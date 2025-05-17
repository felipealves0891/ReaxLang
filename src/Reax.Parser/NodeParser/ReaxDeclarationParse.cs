using Reax.Core.Types;
using Reax.Lexer;
using Reax.Parser.Extensions;
using Reax.Parser.Node;
using Reax.Core.Ast.Statements;
using Reax.Core.Ast;
using Reax.Core.Ast.Expressions;

namespace Reax.Parser.NodeParser;

public class ReaxDeclarationParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.LET || current.Type == TokenType.CONST
           || (current.Type == TokenType.ASYNC && next.Type == TokenType.LET);
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        var isAsync = IsAsync(source);
        var immutable = IsImmutable(source);
        var identifier = GetIdentifier(source);
        var dataType = GetDataType(source);
           
        if(source.CurrentToken.Type == TokenType.SEMICOLON)
        {
            source.Advance();
            return new DeclarationNode(identifier.Source, immutable,  isAsync, dataType, null, identifier.Location);
        }

        source.Advance();
        if(source.NextToken.Type == TokenType.SEMICOLON)
        {
            var value = new AssignmentNode(new VarNode(identifier.Source, dataType, identifier.Location), source.CurrentToken.ToReaxValue(), source.CurrentToken.Location); 
            source.Advance(TokenType.SEMICOLON);
            source.Advance();
            return new DeclarationNode(identifier.Source, immutable,  isAsync, dataType, value, identifier.Location);
        }
        else 
        {
            var assigned = source.NextNode() ?? new NullNode(identifier.Location);
            var value = new AssignmentNode(new VarNode(identifier.Source, dataType, identifier.Location), assigned, assigned.Location);
            return new DeclarationNode(identifier.Source, immutable,  isAsync, dataType, value, identifier.Location);
        }
        
    }

    private bool IsAsync(ITokenSource source)
    {
        var isAsync = source.CurrentToken.Type == TokenType.ASYNC;
        if(isAsync) source.Advance([TokenType.LET]);
        return isAsync;
    }

    private bool IsImmutable(ITokenSource source) 
    {
        var immutable = source.CurrentToken.Type == TokenType.CONST;
        source.Advance([TokenType.IDENTIFIER]);
        return immutable;
    }

    private Token GetIdentifier(ITokenSource source) 
    {
        var identifier = source.CurrentToken;
        source.Advance([TokenType.TYPING]);
        source.Advance(Token.DataTypes);
        return identifier;        
    }

    private DataType GetDataType(ITokenSource source)
    {
        var dataType = source.CurrentToken.Type.ToDataType();
        if (source.NextToken.Type is TokenType.OPEN_BRACKET)
        {
            source.Advance(TokenType.OPEN_BRACKET);
            dataType = dataType | DataType.ARRAY;
            source.Advance(TokenType.CLOSE_BRACKET);
        }

        source.Advance([TokenType.ASSIGNMENT, TokenType.SEMICOLON]);
        return dataType;
    }

}
