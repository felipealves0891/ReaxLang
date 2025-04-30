using Reax.Parser.Node.Interfaces;
using Reax.Runtime;
using Reax.Semantic.Analyzers.TypeChecking;
using Reax.Semantic.Node;

namespace Reax.Parser.Node;

public record FunctionCallNode(
    string Identifier, 
    ReaxNode[] Parameter, 
    SourceLocation Location) : ReaxNode(Location), INodeExpectedType
{
    public bool IsLeaf => true;
    public INode[] Children => [];
    public MultiType ExpectedType { get; set; } = new MultiType(DataType.NONE, DataType.NONE);

    public override string ToString()
    {
        return $"{Identifier}({string.Join(',', Parameter.Select(x => x.ToString()))});";
    }
}
