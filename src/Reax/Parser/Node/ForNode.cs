using Reax.Parser.Helper;
using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record ForNode(
    DeclarationNode Declaration, 
    ReaxNode Condition, 
    ContextNode Block, 
    SourceLocation Location) : ReaxNode(Location), IReaxResult
{
    public override string ToString()
    {
        return $"for {Declaration} to {Condition} {{...}}";
    }

    public IValidateResult Validate(ISemanticContext context, DataType expectedType = DataType.NONE)
    {
        throw new NotImplementedException();
    }
}
