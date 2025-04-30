using Reax.Parser.Node.Interfaces;
using Reax.Semantic.Analyzers.TypeChecking;
using Reax.Semantic.Nodes;

namespace Reax.Parser.Node;

public record FunctionDeclarationNode(
    IdentifierNode Identifier, 
    ContextNode Block, 
    VarNode[] Parameters, 
    DataType SuccessType,
    DataType ErrorType,
    SourceLocation Location) : ReaxNode(Location), INodeExpectedType
{
    public MultiType ExpectedType => new MultiType(SuccessType, ErrorType);
    public bool IsLeaf => false;
    public INode[] Children => [(INode)Block];

    public override string ToString()
    {
        var param = string.Join(',', Parameters.Select(x => x.ToString()));
        return $"fun {Identifier.Identifier} ({param}):{SuccessType} | {ErrorType} {{...}}";
    }
}
