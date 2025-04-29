using Reax.Parser.Node.Interfaces;
using Reax.Runtime;
using Reax.Semantic;

namespace Reax.Parser.Node;

public record FunctionCallNode(
    string Identifier, 
    ReaxNode[] Parameter, 
    SourceLocation Location) : ReaxNode(Location), IReaxResult
{
    public override string ToString()
    {
        return $"{Identifier}({string.Join(',', Parameter.Select(x => x.ToString()))});";
    }

    public IValidateResult Validate(ISemanticContext context, DataType expectedType = DataType.NONE)
    {
        var functionSymbol = context.GetSymbol(Identifier);
        if(functionSymbol is null)
            return ValidationResult.ErrorUndeclared(Identifier, Location);

        var expectedParameters = context.GetParameters(Identifier);
        var requiredParametersCount = expectedParameters.Count(x => x.Category == Semantic.Symbols.SymbolCategory.PARAMETER);
        if (Parameter.Count() < requiredParametersCount)
            return ValidationResult.ErrorParameterCount(Identifier, requiredParametersCount, Parameter.Count(), Location);
        else if(Parameter.Count() > expectedParameters.Length)
            return ValidationResult.ErrorParameterCount(Identifier, expectedParameters.Length, Parameter.Count(), Location);

        for (int i = 0; i < Parameter.Count(); i++)
        {
            if(Parameter[i] is IReaxType reaxType)
            {
                if(reaxType.Type == expectedParameters[i].Type)
                    Results.Add(ValidationResult.Success(Location));
                else
                    Results.Add(ValidationResult.ErrorInvalidType("", Parameter[i].Location));
            }
        }

        if(functionSymbol.Type != expectedType)
            Results.Add(ValidationResult.ErrorInvalidType(Identifier, Location));
        
        return ValidationResult.Join(Results);
    }
}
