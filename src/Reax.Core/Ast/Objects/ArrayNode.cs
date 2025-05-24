using System.Collections;
using System.Collections.Immutable;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Locations;
using Reax.Core.Types;

namespace Reax.Core.Ast.Objects;

public record ArrayNode(ImmutableArray<ReaxNode> Literals, SourceLocation Location)
    : ObjectNode(Location), IEnumerable<ReaxNode>
{
    public ReaxNode this[int i] => Literals[i];

    public override IReaxNode[] Children => Literals.ToArray();
    public override object Value => Literals.ToArray();
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
