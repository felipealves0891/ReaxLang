using Reax.Interpreter;
using Reax.Parser.Node.Interfaces;
using Reax.Semantic;

namespace Reax.Parser.Node;

public record ScriptNode(
    string Identifier, 
    ReaxInterpreter Interpreter, 
    SourceLocation Location) : ReaxNode(Location), IReaxResult
{
    public override string ToString()
    {
        return $"import script {Identifier};";
    }

    public IValidateResult Validate(ISemanticContext context, DataType expectedType = DataType.NONE)
    {
        foreach (var node in Interpreter.Nodes)
        {
            if(node is IReaxResult reaxResult)
            {
                Results.Add(reaxResult.Validate(context, expectedType));
            }
        }

        return ValidationResult.Join(Results);
    }
}