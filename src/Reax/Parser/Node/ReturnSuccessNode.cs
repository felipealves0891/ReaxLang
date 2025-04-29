
using Reax.Parser.Node.Interfaces;
using Reax.Semantic;

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
        if(Expression is IReaxResult reaxResult)
            Results.Add(reaxResult.Validate(context, expectedType));
        else if(Expression is IReaxType reaxType)
        {
            if(expectedType == reaxType.Type)
                Results.Add(ValidationResult.Success(Location));
            else
                Results.Add(ValidationResult.ErrorInvalidType("", Location));
            
        }
        else
            Results.Add(ValidationResult.ErrorNoResultExpression(Location));

        return ValidationResult.Join(Results);
    }
}
