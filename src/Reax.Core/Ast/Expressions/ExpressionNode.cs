using System;
using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;

namespace Reax.Core.Ast.Expressions;

[ExcludeFromCodeCoverage]
public abstract record ExpressionNode(SourceLocation Location)
    : ReaxNode(Location)
{
}
