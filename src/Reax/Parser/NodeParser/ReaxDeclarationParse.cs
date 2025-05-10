using Reax.Lexer;
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
           
        if(source.CurrentToken.Type == TokenType.END_EXPRESSION)
        {
            source.Advance();
            return new DeclarationNode(identifier.Source, immutable,  isAsync, dataType, null, identifier.Location);
        }

        source.Advance();
        if(source.NextToken.Type == TokenType.END_EXPRESSION)
        {
            var value = new AssignmentNode(new VarNode(identifier.Source, dataType, identifier.Location), source.CurrentToken.ToReaxValue(), source.CurrentToken.Location); 
            source.Advance(TokenType.END_EXPRESSION);
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
        source.Advance([TokenType.ASSIGNMENT, TokenType.END_EXPRESSION]);
        return dataType;
    }

}
