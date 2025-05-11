using System;
using Reax.Core.Locations;
using Reax.Parser;

namespace Reax.Semantic;

public interface IReaxNode
{
    SourceLocation Location { get; }
    IReaxNode[] Children { get; }
}
