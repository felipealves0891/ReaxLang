using System;
using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;
using Reax.Lexer;
using Reax.Parser.Node.Interfaces;
using Reax.Parser.Node.Statements;
using Reax.Semantic;

namespace Reax.Parser.Node.Expressions;

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
