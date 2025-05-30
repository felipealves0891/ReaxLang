using System;
using System.Diagnostics.CodeAnalysis;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;
using Reax.Core.Locations;

namespace Reax.Core.Ast.Expressions;

[ExcludeFromCodeCoverage]
public abstract record ExpressionNode(SourceLocation Location)
    : ReaxNode(Location), IReaxExpression
{
    public abstract IReaxValue Evaluation(IReaxExecutionContext context);
}
