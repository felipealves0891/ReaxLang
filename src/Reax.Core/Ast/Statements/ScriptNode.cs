using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;

namespace Reax.Core.Ast.Statements;

[ExcludeFromCodeCoverage]
public record ScriptNode : StatementNode
{
    public ScriptNode(
        string identifier, 
        ReaxNode[] nodes, 
        SourceLocation location) : base(location)
    {
        Identifier = identifier;
        Nodes = nodes;
    }

    public override IReaxNode[] Children => Nodes;

    public string Identifier { get; set; }
    public ReaxNode[] Nodes { get; }

    public override void Accept(IReaxInterpreter interpreter)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return $"import script {Identifier};";
    }
}