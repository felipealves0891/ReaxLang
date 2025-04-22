using System;
using Reax.Parser.Node;

namespace Reax.Semantic.Interfaces;

public interface IReaxChildren
{
    ReaxNode[] Children { get; }
}
