using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;

namespace Reax.Core.Ast.Statements;

[ExcludeFromCodeCoverage]
public record ScriptNode : StatementNode
{
    public ScriptNode(string identifier, ReaxNode[] nodes, SourceLocation Location) : base(Location)
    {
        Identifier = identifier;
        Nodes = nodes;
    }

    public string Identifier { get; set; }
    public ReaxNode[] Nodes { get; init; }

    public override IReaxNode[] Children => Nodes;

    public override void Execute(IReaxExecutionContext context)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return $"import script {Identifier};";
    }
}