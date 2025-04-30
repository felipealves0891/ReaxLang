using Reax.Parser.Node.Interfaces;
using Reax.Semantic.Analyzers.TypeChecking;
using Reax.Semantic.Node;

namespace Reax.Parser.Node;

public record IfNode(
    BinaryNode Condition, 
    ContextNode True, 
    ContextNode? False, 
    SourceLocation Location) : ReaxNode(Location), INode
{
    public bool IsLeaf => false;
    public INode[] Children => False is null ? [(INode)True] : [(INode)True, (INode)False];

    public override string ToString()
    {
        var elseText = False is null ? "" : "else {}";
        return $"if {Condition} {{}} {elseText}";
    }
}
