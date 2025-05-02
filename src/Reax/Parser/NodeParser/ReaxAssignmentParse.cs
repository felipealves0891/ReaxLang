using Reax.Lexer;
using Reax.Parser.Node;
using Reax.Parser.Node.Statements;

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
        
        if(source.NextToken.Type == TokenType.END_STATEMENT)
        {
            source.Advance(TokenType.END_STATEMENT);
            var assignment = new AssignmentNode(var, source.BeforeToken.ToReaxValue(), identifier.Location);
            source.Advance();
            return assignment;
        }
        else 
        {
            var node = source.NextNode() ?? new NullNode(identifier.Location);
            var assigned = new ContextNode([node], node.Location);
            return new AssignmentNode(var, assigned, identifier.Location);
        }
    }
}
