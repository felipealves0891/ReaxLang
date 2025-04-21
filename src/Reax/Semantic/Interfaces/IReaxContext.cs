using System;
using Reax.Parser.Node;
using Reax.Semantic.Symbols;

namespace Reax.Semantic.Interfaces;

public interface IReaxContext
{
    ReaxNode[] Context { get; }
    Symbol[] GetParameters(Guid scope);
}
