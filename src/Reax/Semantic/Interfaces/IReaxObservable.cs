using System;
using Reax.Parser.Node;

namespace Reax.Semantic.Interfaces;

public interface IReaxObservable
{
    string Identifier { get; }
    ReaxNode[] Children { get; }
}
