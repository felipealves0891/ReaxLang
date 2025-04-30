using Reax.Parser.Helper;
using Reax.Parser.Node.Interfaces;
using Reax.Semantic.Nodes;

namespace Reax.Parser.Node;

public record WhileNode(
    ReaxNode condition, 
    ContextNode Block, 
    SourceLocation Location) : ReaxNode(Location), INode
{
    public bool IsLeaf => false;
    public INode[] Children => [(INode)Block];

    public override string ToString()
    {
        return $"while {condition} {{...}}";
    }
}
