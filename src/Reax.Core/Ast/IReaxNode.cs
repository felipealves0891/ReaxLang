using System;
using Reax.Core.Locations;
using Reax.Parser;

namespace Reax.Core.Ast;

public interface IReaxNode
{
    SourceLocation Location { get; }
    IReaxNode[] Children { get; }
    void Serialize(BinaryWriter writer);
}
