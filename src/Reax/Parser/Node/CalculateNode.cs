using Reax.Parser.Node.Interfaces;
using Reax.Runtime;
using Reax.Semantic.Analyzers.TypeChecking;
using Reax.Semantic.Nodes;

namespace Reax.Parser.Node;

public record CalculateNode(
    ReaxNode Left, 
    ReaxNode Operator, 
    ReaxNode Right, 
    SourceLocation Location) : ReaxNode(Location), INodeResultType
{
    public DataType ResultType => DataType.NUMBER;
    public bool IsLeaf => true;
    public INode[] Children => [];

    public override string ToString()
    {
        return $"{Left} {Operator} {Right}";
    }
}
