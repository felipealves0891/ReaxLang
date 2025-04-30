using System;
using Reax.Parser;

namespace Reax.Semantic.Nodes;

public interface INode
{
    bool IsLeaf { get; }
    INode[] Children { get; }
    SourceLocation Location{ get; }
}
