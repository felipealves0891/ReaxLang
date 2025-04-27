using Reax.Parser.Node.Interfaces;

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
        throw new NotImplementedException();
    }
}
