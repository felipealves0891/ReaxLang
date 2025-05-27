using System.Diagnostics.CodeAnalysis;
using Reax.Core.Helpers;
using Reax.Core.Locations;

namespace Reax.Core.Ast.Statements;

[ExcludeFromCodeCoverage]
public record ScriptNode : StatementNode, IReaxDeclaration
{
    public ScriptNode(
        string filename,
        ReaxNode[] nodes,
        SourceLocation Location) : base(Location)
    {
        Nodes = nodes;
        Filename = filename;
    }

    public ScriptNode(
        string filename,
        string identifier,
        ReaxNode[] nodes,
        SourceLocation Location) : base(Location)
    {
        Nodes = nodes;
        Filename = filename;
        Identifier = identifier;
    }

    public string Filename { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public ReaxNode[] Nodes { get; init; }

    public override IReaxNode[] Children => Nodes;

    public override void Execute(IReaxExecutionContext context)
    {
        var interpreter = context.CreateInterpreter(Identifier, Nodes);
        interpreter.Interpret();
        context.SetScript(Identifier, interpreter);
    }

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);

        writer.Write(Filename);
        writer.Write(Identifier);
        writer.Write(Nodes.Length);
        foreach (var node in Nodes)
        {
            node.Serialize(writer);
        }
        base.Serialize(writer);
    }

    public static new ScriptNode Deserialize(BinaryReader reader)
    {
        var filename = reader.ReadString();
        var identifier = reader.ReadString();
        var nodesCount = reader.ReadInt32();
        var nodes = new ReaxNode[nodesCount];

        for (var i = 0; i < nodesCount; i++)
            nodes[i] = BinaryDeserializerHelper.Deserialize<ReaxNode>(reader);
        
        var location = ReaxNode.Deserialize(reader);
        return new ScriptNode(filename, identifier, nodes, location);
    }
    
    public void Initialize(IReaxExecutionContext context)
    {
        context.Declare(Identifier);
    }

    public override string ToString()
    {
        return $"import script {Filename};";
    }
}