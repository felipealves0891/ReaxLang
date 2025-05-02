using Reax.Semantic;

namespace Reax.Parser.Node.Statements;

public record ActionNode(
    VarNode[] Parameters,
    ReaxNode Context,
    DataType Type,
    SourceLocation Location) : StatementNode(Location)
{
    public override IReaxNode[] Children => [Context];

    public override string ToString()
    {
        var parameters = string.Join(',', Parameters.Select(x => x.ToString()));
        return $"({parameters}){Type} -> {{...}}";
    }
}
