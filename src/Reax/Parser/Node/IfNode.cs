using Reax.Parser.Node.Interfaces;
using Reax.Semantic;

namespace Reax.Parser.Node;

public record IfNode(
    BinaryNode Condition, 
    ContextNode True, 
    ContextNode? False, 
    SourceLocation Location) : ReaxNode(Location), IReaxResult
{
    public override string ToString()
    {
        var elseText = False is null ? "" : "else {}";
        return $"if {Condition} {{}} {elseText}";
    }

    public IValidateResult Validate(ISemanticContext context, DataType expectedType = DataType.NONE)
    {
        if(Condition.Type == DataType.BOOLEAN)
            Results.Add(ValidationResult.Success(Condition.Location));
        else
            Results.Add(ValidationResult.ErrorInvalidType("Era esperado uma expressão boleana", Condition.Location));
            
        using(context.EnterScope())
        {
            Results.Add(True.Validate(context, expectedType));
        }

        using(context.EnterScope())
        {
            if(False is not null)
                Results.Add(True.Validate(context, expectedType));
        }        
        
        return ValidationResult.Join(Results);
    }
}
