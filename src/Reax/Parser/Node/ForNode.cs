using Reax.Parser.Helper;
using Reax.Parser.Node.Interfaces;
using Reax.Semantic.Node;

namespace Reax.Parser.Node;

public record ForNode(
    DeclarationNode Declaration, 
    ReaxNode Condition, 
    ContextNode Block, 
    SourceLocation Location) : ReaxNode(Location), INode
{
    public bool IsLeaf => false;
    public INode[] Children => [(INode)Declaration, (INode)Condition, (INode)Block];

    public override string ToString()
    {
        return $"for {Declaration} to {Condition} {{...}}";
    }
}
