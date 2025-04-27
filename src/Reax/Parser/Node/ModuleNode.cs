using Reax.Parser.Node.Interfaces;
using Reax.Runtime.Functions;

namespace Reax.Parser.Node;

public record ModuleNode(
    string identifier, 
    Dictionary<string, Function> functions, 
    SourceLocation Location) : ReaxNode(Location), IReaxResult
{
    public override string ToString()
    {
        return $"import module {identifier};";
    }

    public IValidateResult Validate(ISemanticContext context)
    {
        throw new NotImplementedException();
    }
}
