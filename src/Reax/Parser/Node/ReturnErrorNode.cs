using Reax.Parser.Node.Interfaces;
using Reax.Runtime;

namespace Reax.Parser.Node;

public record ReturnErrorNode(
    ReaxNode Expression, 
    SourceLocation Location) : ReaxNode(Location), IReaxResult
{
    public override string ToString()
    {
        return $"return error {Expression}";
    }

    public IValidateResult Validate(ISemanticContext context, DataType expectedType = DataType.NONE)
    {
        throw new NotImplementedException();
    }
}
