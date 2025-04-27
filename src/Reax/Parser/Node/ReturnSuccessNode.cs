
using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record ReturnSuccessNode(
    ReaxNode Expression, 
    SourceLocation Location) : ReaxNode(Location), IReaxResult
{
    public override string ToString()
    {
        return $"return success {Expression}";
    }

    public IValidateResult Validate(ISemanticContext context, DataType expectedType = DataType.NONE)
    {
        throw new NotImplementedException();
    }
}
