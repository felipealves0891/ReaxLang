using System;
using Reax.Parser.Node.Interfaces;
using Reax.Semantic.Analyzers.TypeChecking;
using Reax.Semantic.Node;

namespace Reax.Parser.Node.Literals;

public abstract record LiteralNode(
    string Source, 
    SourceLocation Location) : ReaxNode(Location), IReaxValue, INodeResultType
{
    public abstract object Value { get; }
    public abstract DataType Type { get; }

    public DataType ResultType => Type;
    public bool IsLeaf => true;
    public INode[] Children => [];
}
