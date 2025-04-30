using System;
using Reax.Lexer;
using Reax.Parser.Node.Interfaces;
using Reax.Semantic.Analyzers.TypeChecking;
using Reax.Semantic.Nodes;

namespace Reax.Parser.Node;

public record MatchNode(
    ReaxNode Expression,    
    ActionNode Success,
    ActionNode Error,  
    SourceLocation Location) : ReaxNode(Location), INodeExpectedType
{
    public MultiType ExpectedType => ((INodeExpectedType)Expression).ExpectedType;
    public bool IsLeaf => false;
    public INode[] Children => [(INode)Success, (INode)Error];

    public override string ToString()
    {
        return $"match {Expression} {{ success, error }}";
    }
}
