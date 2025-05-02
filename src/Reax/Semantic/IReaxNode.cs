using System;
using Reax.Parser;

namespace Reax.Semantic;

public interface IReaxNode
{
    SourceLocation Location { get; }
    IReaxNode[] Children { get; }
}
