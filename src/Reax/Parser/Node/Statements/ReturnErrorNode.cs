using Reax.Parser.Node.Interfaces;
using Reax.Runtime;
using Reax.Semantic;

namespace Reax.Parser.Node.Statements;

public record ReturnErrorNode(
    ReaxNode Expression, 
    SourceLocation Location) : StatementNode(Location)
{
    public override IReaxNode[] Children => [Expression];

    public override string ToString()
    {
        return $"return error {Expression}";
    }
}
