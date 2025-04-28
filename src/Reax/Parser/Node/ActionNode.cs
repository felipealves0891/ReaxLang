using System;
using Reax.Parser.Node.Interfaces;
using Reax.Semantic;
using Reax.Semantic.Symbols;

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
        foreach (var parameter in Parameters)
        {
            var symbol = new Symbol(parameter.Identifier, parameter.Type, SymbolCategory.PARAMETER, parameter.Location);
            Results.Add(context.SetSymbol(symbol));
        }

        if(Context is IReaxType reaxType)
        {
            if(reaxType.Type == expectedType)
                Results.Add(ValidationResult.Success(Location));
            else
                Results.Add(ValidationResult.ErrorInvalidType(ToString(), Location));
        }
        else if(Context is IReaxResult reaxResult)
            Results.Add(reaxResult.Validate(context, expectedType));
        else 
            Results.Add(ValidationResult.ErrorNoResultExpression(Location));

        return ValidationResult.Join(Results);
            
    }
}
