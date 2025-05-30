using Reax.Core.Types;
using Reax.Lexer;
using Reax.Parser.Extensions;
using Reax.Parser.Node;
using Reax.Core.Ast.Statements;
using Reax.Core.Ast;
using Reax.Core.Ast.Expressions;

namespace Reax.Parser.NodeParser;

public class ReaxAssignmentParse : INodeParser
{
    public bool IsParse(Token before, Token current, Token next)
    {
        return current.Type == TokenType.IDENTIFIER 
            && next.Type == TokenType.ASSIGNMENT;
    }

    public ReaxNode? Parse(ITokenSource source)
    {
        var identifier = source.CurrentToken;
        var var = new VarNode(identifier.Source, DataType.NONE, identifier.Location);

        source.Advance(TokenType.ASSIGNMENT);
        source.Advance();
        
        if(source.NextToken.Type == TokenType.SEMICOLON)
        {
            source.Advance(TokenType.SEMICOLON);
            var assignment = new AssignmentNode(var, source.BeforeToken.ToReaxValue(), identifier.Location);
            source.Advance();
            return assignment;
        }
        else 
        {
            var node = source.NextNode() ?? new NullNode(identifier.Location);
            return new AssignmentNode(var, node, identifier.Location);
        }
    }
}
