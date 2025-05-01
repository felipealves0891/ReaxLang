using Reax.Parser.Node.Interfaces;
using Reax.Runtime;

namespace Reax.Parser.Node.Statements;

public record ReturnErrorNode(
    ReaxNode Expression, 
    SourceLocation Location) : StatementNode(Location)
{
    public override string ToString()
    {
        return $"return error {Expression}";
    }
}
