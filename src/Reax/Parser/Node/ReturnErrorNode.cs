using Reax.Parser.Node.Interfaces;
using Reax.Runtime;
using Reax.Semantic.Node;

namespace Reax.Parser.Node;

public record ReturnErrorNode(
    ReaxNode Expression, 
    SourceLocation Location) : ReaxNode(Location), INode
{
    public bool IsLeaf => false;
    public INode[] Children => [(INode)Expression];

    public override string ToString()
    {
        return $"return error {Expression}";
    }
}
