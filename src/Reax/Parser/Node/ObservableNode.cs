using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record ObservableNode(
    VarNode Var, 
    ContextNode Block, 
    BinaryNode? Condition, 
    SourceLocation Location) : ReaxNode(Location), IReaxResult
{
    public override string ToString()
    {
        var when = Condition is null ? "" : $"whe {Condition} "; 
        return $"on {Var} {when}{{...}}";
    }

    public IValidateResult Validate(ISemanticContext context, DataType expectedType = DataType.NONE)
    {
        throw new NotImplementedException();
    }
}
