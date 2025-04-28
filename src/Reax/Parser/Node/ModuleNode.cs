using Reax.Parser.Node.Interfaces;
using Reax.Runtime.Functions;
using Reax.Semantic;
using Reax.Semantic.Symbols;

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

    public IValidateResult Validate(ISemanticContext context, DataType expectedType = DataType.NONE)
    {
        var module = new Symbol(identifier, DataType.NONE, SymbolCategory.MODULE, Location);
        Results.Add(context.SetSymbol(module));

        foreach (var key in functions.Keys)
        {
            var fun = functions[key];
            if(fun is IReaxResult decorate)
            {
                Results.Add(decorate.Validate(context)); 
            }   
        }

        return ValidationResult.Join(Results);
    }
}
