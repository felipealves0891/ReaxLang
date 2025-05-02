using Reax.Parser.Node.Interfaces;
using Reax.Parser.Node.Statements;
using Reax.Runtime;
using Reax.Semantic;

namespace Reax.Parser.Node.Statements;

public record ContextNode(
    ReaxNode[] Block, 
    SourceLocation Location) : StatementNode(Location)
{
    public override IReaxNode[] Children => Block;

    public override string ToString()
    {
        return "{...}";
    }
}
