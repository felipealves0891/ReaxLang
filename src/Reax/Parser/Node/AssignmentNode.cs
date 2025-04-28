using Reax.Parser.Node.Interfaces;
using Reax.Semantic;

namespace Reax.Parser.Node;

public record AssignmentNode(
    VarNode Identifier, 
    ReaxNode Assigned, 
    SourceLocation Location) : ReaxNode(Location), IReaxResult
{
    public override string ToString()
    {
        return $"{Identifier} = {Assigned};";
    }

    public IValidateResult Validate(ISemanticContext context, DataType expectedType = DataType.NONE)
    {
        var symbol = context.GetSymbol(Identifier.Identifier);
        if(symbol is null)
            Results.Add(ValidationResult.ErrorUndeclared(Identifier.Identifier, Location));

        var type = symbol is not null ? symbol.Type : expectedType;
        if(Assigned is IReaxType reaxType)
        {
            if(reaxType.Type == type)
                Results.Add(ValidationResult.Success(Location));
            else
                Results.Add(ValidationResult.ErrorInvalidType(Identifier.Identifier, Location));
        }
        else if(Assigned is IReaxResult reaxResult) 
        {
            Results.Add(reaxResult.Validate(context, type));
        }

        return ValidationResult.Join(Results);
    }
}
