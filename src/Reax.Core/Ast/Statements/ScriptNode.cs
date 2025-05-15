using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;

namespace Reax.Core.Ast.Statements;

[ExcludeFromCodeCoverage]
public record ScriptNode(
    string Identifier, 
    ReaxNode[] Nodes, 
    SourceLocation Location) : StatementNode(Location)
{
    public override IReaxNode[] Children => Nodes;

    public override void Accept(IReaxInterpreter interpreter)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return $"import script {Identifier};";
    }
}