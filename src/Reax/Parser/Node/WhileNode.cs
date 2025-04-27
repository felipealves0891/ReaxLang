using Reax.Parser.Helper;
using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record WhileNode(
    ReaxNode condition, 
    ContextNode Block, 
    SourceLocation Location) : ReaxNode(Location), IReaxResult
{
    public override string ToString()
    {
        return $"while {condition} {{...}}";
    }

    public IValidateResult Validate(ISemanticContext context)
    {
        throw new NotImplementedException();
    }
}
