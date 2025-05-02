using System;

namespace Reax.Semantic;

public interface IReaxNode
{
    IReaxNode[] Children { get; }
}
