using System.Collections;
using System.Collections.Immutable;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Locations;
using Reax.Core.Types;

namespace Reax.Core.Ast.Objects;

public record ArrayNode(ImmutableArray<ReaxNode> Literals, SourceLocation Location)
    : ObjectNode(Location), IEnumerable<ReaxNode>
{
    public override IReaxNode[] Children => Literals.ToArray();
    public override object Value => Literals.ToArray();
    public override DataType Type => DataType.ARRAY;

    public ReaxNode this[int i] => Literals[i];
    public int Length = Literals.Length;

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
