using Reax.Parser.Node.Interfaces;
using Reax.Semantic.Nodes;

namespace Reax.Parser.Node;

public record ObservableNode(
    VarNode Var, 
    ContextNode Block, 
    BinaryNode? Condition, 
    SourceLocation Location) : ReaxNode(Location), INode
{
    public bool IsLeaf => false;
    public INode[] Children => [(INode)Block];

    public override string ToString()
    {
        var when = Condition is null ? "" : $"whe {Condition} "; 
        return $"on {Var} {when}{{...}}";
    }
}
