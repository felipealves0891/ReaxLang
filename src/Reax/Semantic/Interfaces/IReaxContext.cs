using System;
using Reax.Parser.Node;

namespace Reax.Semantic.Interfaces;

public interface IReaxContext
{
    ReaxNode[] Context { get; }
}
