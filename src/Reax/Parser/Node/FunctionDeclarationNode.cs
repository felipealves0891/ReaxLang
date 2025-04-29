using Reax.Parser.Node.Interfaces;
using Reax.Semantic;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record FunctionDeclarationNode(
    IdentifierNode Identifier, 
    ContextNode Block, 
    VarNode[] Parameters, 
    DataType SuccessType,
    DataType ErrorType,
    SourceLocation Location) : ReaxNode(Location), IReaxResult
{
    public override string ToString()
    {
        var param = string.Join(',', Parameters.Select(x => x.ToString()));
        return $"fun {Identifier.Identifier} ({param}):{SuccessType} | {ErrorType} {{...}}";
    }

    public IValidateResult Validate(ISemanticContext context, DataType expectedType = DataType.NONE)
    {
        var symbol = new Symbol(Identifier.Identifier, SuccessType, SymbolCategory.FUNCTION, Location, errorType: ErrorType);
        Results.Add(context.SetSymbol(symbol));

        foreach (var parameter in Parameters)
        {
            var parameterSymbol = new Symbol(parameter.Identifier, parameter.Type, SymbolCategory.PARAMETER, Location, Identifier.Identifier);
            Results.Add(context.SetSymbol(parameterSymbol));
        }
        
        using(context.EnterScope())
        {
            Results.Add(Block.Validate(context, expectedType));
        }
        
        return ValidationResult.Join(Results);
    }
}
