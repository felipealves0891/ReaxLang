using System;
using Reax.Debugger;
using Reax.Lexer;
using Reax.Parser.Node;

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
        DeclarationNode? declaration = null;

        var isAsync = source.CurrentToken.Type == TokenType.ASYNC;
        if(isAsync) source.Advance([TokenType.LET]);

        var immutable = source.CurrentToken.Type == TokenType.CONST;
        source.Advance([TokenType.IDENTIFIER]);

        var identifier = source.CurrentToken;
        source.Advance([TokenType.TYPING]);
        source.Advance(Token.DataTypes);

        var dataType = new DataTypeNode(source.CurrentToken.Source, source.CurrentToken.Location);  
        source.Advance([TokenType.ASSIGNMENT, TokenType.END_STATEMENT]);
        
        if(source.CurrentToken.Type == TokenType.END_STATEMENT)
        {
            declaration = new DeclarationNode(identifier.Source, immutable,  isAsync, dataType, null, identifier.Location);
            Logger.LogParse(declaration.ToString());
            source.Advance();
            return declaration;
        }

        source.Advance();
        if(source.NextToken.Type == TokenType.END_STATEMENT)
        {
            var value = source.CurrentToken.ToReaxValue();
            declaration = new DeclarationNode(identifier.Source, immutable,  isAsync, dataType, value, identifier.Location);
            source.Advance(TokenType.END_STATEMENT);
            source.Advance();
        }
        else 
        {
            var assigned = source.NextNode() ?? new NullNode(identifier.Location);
            var value = new ContextNode([assigned], assigned.Location);
            declaration = new DeclarationNode(identifier.Source, immutable,  isAsync, dataType, value, identifier.Location);
        }
        
        Logger.LogParse(declaration.ToString());
        return declaration;
    }

}
