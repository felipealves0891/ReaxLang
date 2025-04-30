using Reax.Parser.Node.Interfaces;
using Reax.Runtime;
using Reax.Semantic.Node;

namespace Reax.Parser.Node;

public record ContextNode(
    ReaxNode[] Block, 
    SourceLocation Location) : ReaxNode(Location), INode
{
    public bool IsLeaf => true;

    public INode[] Children => Block.Cast<INode>().ToArray();

    public override string ToString()
    {
        return "{...}";
    }
}
