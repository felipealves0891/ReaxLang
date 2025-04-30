using Reax.Parser.Node.Interfaces;
using Reax.Semantic.Analyzers.TypeChecking;
using Reax.Semantic.Node;

namespace Reax.Parser.Node;

public record AssignmentNode(
    VarNode Identifier, 
    ReaxNode Assigned, 
    SourceLocation Location) : ReaxNode(Location), INodeExpectedType
{
    public MultiType ExpectedType => new MultiType(Identifier.Type, Identifier.Type);
    public bool IsLeaf => false;
    public INode[] Children => [(INode)Assigned];

    public override string ToString()
    {
        return $"{Identifier} = {Assigned};";
    }
}
