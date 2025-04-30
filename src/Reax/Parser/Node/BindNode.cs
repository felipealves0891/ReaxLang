using System;
using Reax.Parser.Node.Interfaces;
using Reax.Semantic.Analyzers.TypeChecking;
using Reax.Semantic.Nodes;

namespace Reax.Parser.Node;

public record BindNode(
    IdentifierNode Identifier, 
    ContextNode Node,
    DataType Type, 
    SourceLocation Location) : ReaxNode(Location), INodeExpectedType
{
    public MultiType ExpectedType => new MultiType(Type, Type);
    public bool IsLeaf => false;
    public INode[] Children => [(INode)Node];

    public override string ToString()
    {
        return $"bind {Identifier}: {Type} -> {{...}}";
    }
}
