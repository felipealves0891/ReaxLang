using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;
using Reax.Core.Ast.Interfaces;

namespace Reax.Core.Ast.Expressions;

[ExcludeFromCodeCoverage]
public record FunctionCallNode(
    string Identifier, 
    ReaxNode[] Parameter, 
    SourceLocation Location) : ExpressionNode(Location)
{
    public override IReaxNode[] Children => Parameter;

    public override string ToString()
    {
        return $"{Identifier}({string.Join(',', Parameter.Select(x => x.ToString()))});";
    }
}
