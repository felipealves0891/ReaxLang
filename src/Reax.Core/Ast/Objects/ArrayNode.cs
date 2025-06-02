using System.Collections;
using System.Collections.Immutable;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Helpers;
using Reax.Core.Locations;
using Reax.Core.Types;

namespace Reax.Core.Ast.Objects;

public record ArrayNode(
    ImmutableArray<ReaxNode> Literals,
    SourceLocation Location)
    : ObjectNode(Location), IEnumerable<ReaxNode>
{
    public ReaxNode this[int i] => Literals[i];

    public override IReaxNode[] Children => Literals.ToArray();
    public override object Value => Literals.Select(x => ((IReaxValue)x).Value).ToArray();
    public override DataType Type => DataType.ARRAY;
    public int Length = Literals.Length;
    
    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);

        writer.Write(Literals.Length);
        foreach (var literal in Literals)
        {
            literal.Serialize(writer);
        }
        base.Serialize(writer);
    }

    public static new ArrayNode Deserialize(BinaryReader reader)
    {
        var length = reader.ReadInt32();
        var literals = new ReaxNode[length];
        for (var i = 0; i < length; i++)
        {
            literals[i] = BinaryDeserializerHelper.Deserialize<ReaxNode>(reader);
        }
        var location = ReaxNode.Deserialize(reader);
        return new ArrayNode(literals.ToImmutableArray(), location);
    }

    public IEnumerator<ReaxNode> GetEnumerator()
    {
        return Literals.ToList().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override string ToString()
    {
        if (Literals.Length > 10)
            return $"[{string.Join(',', Literals.Take(10))}]";
        else
            return $"[{string.Join(',', Literals)}]";
    }
}
