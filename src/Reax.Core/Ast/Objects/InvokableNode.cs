using System;
using Reax.Core.Helpers;
using Reax.Core.Locations;
using Reax.Core.Types;

namespace Reax.Core.Ast.Objects;

public record InvokableNode : ObjectNode
{
    public InvokableNode(
        ReaxNode node,
        SourceLocation location)
        : base(location)
    {
        Value = node;
    }

    public override object Value { get; }
    public override DataType Type => DataType.NONE;
    public override IReaxNode[] Children => [(ReaxNode)Value];
    
    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);
        ((IReaxNode)Value).Serialize(writer);
        base.Serialize(writer);
    }

    public static new InvokableNode Deserialize(BinaryReader reader)
    {
        var node = BinaryDeserializerHelper.Deserialize<ReaxNode>(reader);
        var location = ReaxNode.Deserialize(reader);
        return new InvokableNode(node, location);
    }

    public override string ToString()
    {
        return $"invokable {Value}";
    }
}
