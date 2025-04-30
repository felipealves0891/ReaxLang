using Reax.Parser.Node.Interfaces;
using Reax.Semantic.Node;

namespace Reax.Parser.Node;

public record ReturnSuccessNode(
    ReaxNode Expression, 
    SourceLocation Location) : ReaxNode(Location), INode
{
    public bool IsLeaf => false;
    public INode[] Children => [(INode)Expression];

    public override string ToString()
    {
        return $"return success {Expression}";
    }
}
