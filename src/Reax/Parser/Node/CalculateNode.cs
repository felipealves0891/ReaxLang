using Reax.Parser.Node.Interfaces;
using Reax.Runtime;
using Reax.Semantic;

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
        Results.Add(Validate(context, expectedType, Left));
        Results.Add(Validate(context, expectedType, Right));
        return ValidationResult.Join(Results);
    }

    private IValidateResult Validate(ISemanticContext context, DataType expectedType, ReaxNode node) 
    {
        if(node is IReaxResult reaxResult)
        {
            return reaxResult.Validate(context, expectedType);
        }
        else if(node is IReaxType reaxType)
        {
            if(reaxType.Type == expectedType)
                return ValidationResult.Success(node.Location);
            else
                return ValidationResult.ErrorInvalidType("", node.Location);
        }
        else
        {
            return ValidationResult.ErrorNoResultExpression(node.Location);
        }
            
    }
}
