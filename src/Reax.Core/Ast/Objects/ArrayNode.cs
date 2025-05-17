using System.Collections;
using System.Collections.Immutable;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Locations;

namespace Reax.Core.Ast.Objects;

public record ArrayNode(ImmutableArray<ReaxNode> Literals, SourceLocation Location)
    : ObjectNode(Location), IReaxValue, IEnumerable<ReaxNode>
{
    public override IReaxNode[] Children => Literals.ToArray();

    public object Value => Literals.ToArray();

    public ReaxNode this[int i] => Literals[i];

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
