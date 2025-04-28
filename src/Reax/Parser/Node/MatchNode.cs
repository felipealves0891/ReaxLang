using System;
using Reax.Lexer;
using Reax.Parser.Node.Interfaces;
using Reax.Semantic;

namespace Reax.Parser.Node;

public record MatchNode(
    ReaxNode Expression,    
    ActionNode Success,
    ActionNode Error,  
    SourceLocation Location) : ReaxNode(Location), IReaxResult
{
    public override string ToString()
    {
        return $"match {Expression} {{ success, error }}";
    }

    public IValidateResult Validate(ISemanticContext context, DataType expectedType = DataType.NONE)
    {
        if(Expression is IReaxType expressionValue)
            ValidateExpressionValue(expressionValue, expectedType);
        else if(Expression is IReaxResult expressionResult)
            Results.Add(expressionResult.Validate(context, expectedType));
        else
            Results.Add(ValidationResult.ErrorNoResultExpression(Expression.Location));
        
        Results.Add(Success.Validate(context, expectedType));
        Results.Add(Error.Validate(context, expectedType));

        return ValidationResult.Join(Results);
    }

    private void ValidateExpressionValue(IReaxType expressionValue, DataType expectedType) 
    {
        if(expressionValue.Type == expectedType)
            Results.Add(ValidationResult.Success(new SourceLocation()));
        else
            Results.Add(ValidationResult.ErrorInvalidType(Expression.ToString(), new SourceLocation()));
    }
}
