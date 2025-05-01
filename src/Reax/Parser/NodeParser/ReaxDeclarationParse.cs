using Reax.Lexer;
using Reax.Parser.Helper;
using Reax.Parser.Node;
using Reax.Parser.Node.Statements;

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
           
        if(source.CurrentToken.Type == TokenType.END_STATEMENT)
        {
            SymbolHelper.Register(identifier, immutable, isAsync, dataType);
            source.Advance();
            return new DeclarationNode(identifier.Source, immutable,  isAsync, dataType, null, identifier.Location);
        }

        source.Advance();
        if(source.NextToken.Type == TokenType.END_STATEMENT)
        {
            var value = source.CurrentToken.ToReaxValue();
            source.Advance(TokenType.END_STATEMENT);
            source.Advance();
            SymbolHelper.RegisterAndAssign(identifier, immutable, isAsync, dataType, value.Location);
            return new DeclarationNode(identifier.Source, immutable,  isAsync, dataType, value, identifier.Location);
        }
        else 
        {
            var assigned = source.NextNode() ?? new NullNode(identifier.Location);
            var value = new ContextNode([assigned], assigned.Location);
            SymbolHelper.RegisterAndAssign(identifier, immutable, isAsync, dataType, value.Location);
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
        source.Advance([TokenType.ASSIGNMENT, TokenType.END_STATEMENT]);
        return dataType;
    }

}
