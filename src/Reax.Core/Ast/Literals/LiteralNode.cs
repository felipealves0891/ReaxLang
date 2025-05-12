using System;
using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;
using Reax.Core.Types;
using Reax.Core.Ast.Interfaces;

namespace Reax.Core.Ast.Literals;

[ExcludeFromCodeCoverage]
public abstract record LiteralNode(
    string Source, 
    SourceLocation Location) : ReaxNode(Location), IReaxValue
{
    public abstract object Value { get; }
    public abstract DataType Type { get; }
}
