using System;
using Reax.Parser.Node;

namespace Reax.Semantic.Interfaces;

public interface IReaxBinder
{
    string Identifier { get; }
    IReaxChildren Bound { get; }
}
