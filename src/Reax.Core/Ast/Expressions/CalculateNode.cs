using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;

namespace Reax.Core.Ast.Expressions;

[ExcludeFromCodeCoverage]
public record CalculateNode(
    ReaxNode Left, 
    ReaxNode Operator, 
    ReaxNode Right, 
    SourceLocation Location) : ExpressionNode(Location)
{
    public override IReaxNode[] Children => [Left, Operator, Right];

    public override string ToString()
    {
        return $"{Left} {Operator} {Right}";
    }
}
