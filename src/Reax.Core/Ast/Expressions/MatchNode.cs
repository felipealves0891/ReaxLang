using System;
using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Statements;

namespace Reax.Core.Ast.Expressions;

[ExcludeFromCodeCoverage]
public record MatchNode(
    ReaxNode Expression,    
    ActionNode Success,
    ActionNode Error,  
    SourceLocation Location) : ExpressionNode(Location)
{
    public override IReaxNode[] Children => [Expression, Success, Error];

    public override string ToString()
    {
        return $"match {Expression} {{ success, error }}";
    }
}
