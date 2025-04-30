using System;
using Reax.Parser;
using Reax.Semantic.Node;

namespace Reax.Semantic.Analyzers.TypeChecking;

public interface INodeResultType : INode
{
    DataType ResultType { get; }
}
