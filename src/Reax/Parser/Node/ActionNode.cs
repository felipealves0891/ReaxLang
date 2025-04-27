using System;

namespace Reax.Parser.Node;

public record ActionNode(
    VarNode[] Parameters,
    ReaxNode Context,
    DataType Type,
    SourceLocation Location) : ReaxNode(Location)
{
    public ReaxNode[] Children => [Context];

    public override string ToString()
    {
        var parameters = string.Join(',', Parameters.Select(x => x.ToString()));
        return $"({parameters}){Type} -> {{...}}";
    }
}
