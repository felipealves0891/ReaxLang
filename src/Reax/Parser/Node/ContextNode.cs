using Reax.Parser.Node.Interfaces;
using Reax.Runtime;
using Reax.Semantic;

namespace Reax.Parser.Node;

public record ContextNode(
    ReaxNode[] Block, 
    SourceLocation Location) : ReaxNode(Location), IReaxResult
{
    public override string ToString()
    {
        return "{...}";
    }

    public IValidateResult Validate(ISemanticContext context, DataType expectedType = DataType.NONE)
    {
        foreach (var node in Block)
        {
            if(node is IReaxResult reaxResult)
            {
                Results.Add(reaxResult.Validate(context, expectedType));
            }
        }

        return ValidationResult.Join(Results);
    }
}
