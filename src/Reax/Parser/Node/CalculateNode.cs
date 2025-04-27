using Reax.Parser.Node.Interfaces;
using Reax.Runtime;

namespace Reax.Parser.Node;

public record CalculateNode(
    ReaxNode Left, 
    ReaxNode Operator, 
    ReaxNode Right, 
    SourceLocation Location) : ReaxNode(Location), IReaxResult
{
    public override string ToString()
    {
        return $"{Left} {Operator} {Right}";
    }

    public IValidateResult Validate(ISemanticContext context, DataType expectedType = DataType.NONE)
    {
        throw new NotImplementedException();
    }
}
