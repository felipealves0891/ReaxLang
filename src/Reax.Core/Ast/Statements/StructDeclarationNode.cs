using System;
using Reax.Core.Locations;

namespace Reax.Core.Ast.Statements;

public record StructDeclarationNode(
    string Name,
    List<StructFieldNode> Properties,
    SourceLocation Location) : StatementNode(Location)
{
    public override IReaxNode[] Children => [];

    public override void Execute(IReaxExecutionContext context)
    {}

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);

        writer.Write(Name);
        writer.Write(Properties.Count);
        foreach (var property in Properties)
        {
            property.Serialize(writer);
        }
        base.Serialize(writer);
    }

    public override string ToString()
    {
        var properties = string.Join(',', Properties.Select(x => x.ToString()));
        return $"struct {Name} {{ {properties} }}";
    }
}
