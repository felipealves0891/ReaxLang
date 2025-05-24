using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;

namespace Reax.Core.Ast.Statements;

[ExcludeFromCodeCoverage]
public record ScriptNode : StatementNode, IReaxDeclaration
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
        var interpreter = context.CreateInterpreter(Identifier, Nodes);
        interpreter.Interpret();
        
        context.Declare(Identifier);
        context.SetScript(Identifier, interpreter);
    }

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);

        writer.Write(Identifier);
        writer.Write(Nodes.Length);
        foreach (var node in Nodes)
        {
            node.Serialize(writer);
        }
        base.Serialize(writer);
    }

    public override string ToString()
    {
        return $"import script {Identifier};";
    }
}