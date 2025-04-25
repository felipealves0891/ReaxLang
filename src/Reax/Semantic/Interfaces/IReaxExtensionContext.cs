using System;
using Reax.Parser.Node;

namespace Reax.Semantic.Interfaces;

public interface IReaxExtensionContext
{
    string Identifier { get; }
    ReaxNode[] Context { get; }
}
