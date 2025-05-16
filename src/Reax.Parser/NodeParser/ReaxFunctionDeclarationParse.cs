using Reax.Core.Types;
using Reax.Lexer;
using Reax.Parser.Helper;
using Reax.Parser.Node;
using Reax.Core.Ast.Statements;
using Reax.Core.Ast;

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
        var identifier = source.CurrentToken;
        source.Advance(TokenType.OPEN_PARENTHESIS);
        var parameters = ParameterHelper.GetParameters(source).ToArray();
        source.Advance(Token.DataTypes);
        var successType = source.CurrentToken.Type.ToDataType();
        source.Advance([TokenType.PIPE, TokenType.OPEN_BRACE]);

        var errorType = DataType.VOID;
        if(source.CurrentToken.Type == TokenType.PIPE)
        {
            source.Advance(Token.DataTypes);
            errorType = source.CurrentToken.Type.ToDataType();
            source.Advance(TokenType.OPEN_BRACE);
        }

        var block = (ContextNode)source.NextBlock();
        return new FunctionDeclarationNode(identifier.Source, block, parameters, successType, errorType, identifier.Location);
    }
}
