using System;

namespace Reax.Parser.Node.Interfaces;

public interface IReaxContext
{
    ReaxNode[] Branchs { get; }
}
