using Reax.Semantic.Analyzers.TypeChecking;
using Reax.Semantic.Nodes;

namespace Reax.Parser.Node;

public record ActionNode(
    VarNode[] Parameters,
    ReaxNode Context,
    DataType Type,
    SourceLocation Location) : ReaxNode(Location), INodeResultType, INodeExpectedType
{
    public DataType ResultType => Type;
    public bool IsLeaf => false;
    public INode[] Children => [(INode)Context];
    public MultiType ExpectedType => new MultiType(Type, Type);

    public override string ToString()
    {
        var parameters = string.Join(',', Parameters.Select(x => x.ToString()));
        return $"({parameters}){Type} -> {{...}}";
    }
}
