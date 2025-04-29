using System;

namespace Reax.Semantic.Node;

public interface INode
{
    bool IsLeaf { get; }
    INode[] Children { get; }
}
