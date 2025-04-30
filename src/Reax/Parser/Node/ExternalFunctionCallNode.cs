using Reax.Parser.Node.Interfaces;
using Reax.Runtime;
using Reax.Semantic.Analyzers.TypeChecking;
using Reax.Semantic.Node;

namespace Reax.Parser.Node;

public record ExternalFunctionCallNode(
    string scriptName, 
    FunctionCallNode functionCall, 
    SourceLocation Location) : ReaxNode(Location), INodeExpectedType
{
    public bool IsLeaf => false;
    public INode[] Children => [(INode)functionCall];
    public MultiType ExpectedType => ((INodeExpectedType)functionCall).ExpectedType;

    public override string ToString()
    {
        var parameters = functionCall.Parameter.Select(x => x.ToString());
        return $"{scriptName}.{functionCall.Identifier}({string.Join(',', parameters)})";
    }
}
