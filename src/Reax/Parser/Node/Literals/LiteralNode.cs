using System;
using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node.Literals;

public abstract record LiteralNode(
    string Source, 
    SourceLocation Location) : ReaxNode(Location), IReaxValue
{
    public abstract object Value { get; }
}
