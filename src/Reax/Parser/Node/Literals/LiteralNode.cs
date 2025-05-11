using System;
using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;
using Reax.Core.Types;
using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node.Literals;

[ExcludeFromCodeCoverage]
public abstract record LiteralNode(
    string Source, 
    SourceLocation Location) : ReaxNode(Location), IReaxValue
{
    public abstract object Value { get; }
    public abstract DataType Type { get; }
}
