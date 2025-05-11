using System;
using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;
using Reax.Semantic;

namespace Reax.Parser.Node.Expressions;

[ExcludeFromCodeCoverage]
public abstract record ExpressionNode(SourceLocation Location)
    : ReaxNode(Location)
{
}
