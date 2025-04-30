using System;
using Reax.Parser;
using Reax.Semantic.Node;

namespace Reax.Semantic.Analyzers.TypeChecking;

public interface INodeExpectedType : INode
{
    MultiType ExpectedType { get; }
}
