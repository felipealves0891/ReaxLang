using System;
using Reax.Core.Helpers;
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

    public static new StructDeclarationNode Deserialize(BinaryReader reader)
    {
        var name = reader.ReadString();
        var propertyCount = reader.ReadInt32();
        var properties = new List<StructFieldNode>(propertyCount);
        
        for (int i = 0; i < propertyCount; i++)
        {
            properties.Add(BinaryDeserializerHelper.Deserialize<StructFieldNode>(reader));
        }

        var location = ReaxNode.Deserialize(reader);
        return new StructDeclarationNode(name, properties, location);
    }

    public override string ToString()
    {
        var properties = string.Join(',', Properties.Select(x => x.ToString()));
        return $"struct {Name} {{ {properties} }}";
    }
}
