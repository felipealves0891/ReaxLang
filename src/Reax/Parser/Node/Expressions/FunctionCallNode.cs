using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;
using Reax.Parser.Node.Interfaces;
using Reax.Runtime;
using Reax.Semantic;

namespace Reax.Parser.Node.Expressions;

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
