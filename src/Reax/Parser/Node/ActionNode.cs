using System;
using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record ActionNode(
    VarNode[] Parameters,
    ReaxNode Context,
    DataType Type,
    SourceLocation Location) : ReaxNode(Location), IReaxResult
{
    public override string ToString()
    {
        var parameters = string.Join(',', Parameters.Select(x => x.ToString()));
        return $"({parameters}){Type} -> {{...}}";
    }

    public IValidateResult Validate(ISemanticContext context, DataType expectedType = DataType.NONE)
    {
        throw new NotImplementedException();
    }
}
