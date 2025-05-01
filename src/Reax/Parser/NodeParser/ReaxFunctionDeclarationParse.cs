using Reax.Lexer;
using Reax.Parser.Helper;
using Reax.Parser.Node;
using Reax.Parser.Node.Statements;

namespace Reax.Parser.NodeParser;

public class ReaxFunctionDeclarationParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.FUNCTION;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        source.Advance(TokenType.IDENTIFIER);
        var token = source.CurrentToken;
        var identifier = new IdentifierNode(source.CurrentToken.Source, source.CurrentToken.Location);
        source.Advance(TokenType.START_PARAMETER);
        var parameters = ParameterHelper.GetParameters(source).ToArray();
        source.Advance(Token.DataTypes);
        var successType = source.CurrentToken.Type.ToDataType();
        source.Advance([TokenType.PIPE, TokenType.START_BLOCK]);

        var errorType = DataType.VOID;
        if(source.CurrentToken.Type == TokenType.PIPE)
        {
            source.Advance(Token.DataTypes);
            errorType = source.CurrentToken.Type.ToDataType();
            source.Advance(TokenType.START_BLOCK);
        }

        SymbolHelper.FunctionDeclaration(token, successType | errorType, parameters);
        var block = (ContextNode)source.NextBlock();
        return new FunctionDeclarationNode(identifier, block, parameters, successType, errorType, identifier.Location);
    }
}
