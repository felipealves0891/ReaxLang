using System.Diagnostics.CodeAnalysis;
using Reax.Runtime.Functions;
using Reax.Semantic;

namespace Reax.Parser.Node.Statements;

[ExcludeFromCodeCoverage]
public record ModuleNode(
    string identifier, 
    Dictionary<string, Function> functions, 
    SourceLocation Location) : StatementNode(Location)
{
    public override IReaxNode[] Children => [];

    public override string ToString()
    {
        return $"import module {identifier};";
    }
}
