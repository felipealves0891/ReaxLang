using System;

namespace Reax.Parser.Node.Interfaces;

public interface IReaxContext
{
    ReaxNode[] Nodes { get; }
}
