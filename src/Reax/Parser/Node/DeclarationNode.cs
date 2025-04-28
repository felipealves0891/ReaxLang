using Reax.Parser.Node.Interfaces;
using Reax.Semantic;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record DeclarationNode(
    string Identifier, 
    bool Immutable, 
    bool Async, 
    DataType Type,
    ReaxNode? Assignment, 
    SourceLocation Location) : ReaxNode(Location), IReaxResult
{
    public override string ToString()
    {
        var asc = Async ? "async " : "";
        var mut = Immutable ? "const" : "let";
        if(Assignment is not null)
            return $"{asc}{mut} {Identifier}: {Type} = {Assignment};";
        else 
            return $"{asc}{mut} {Identifier}: {Type};";
    }

    public IValidateResult Validate(ISemanticContext context, DataType expectedType = DataType.NONE)
    {
        var category = Immutable ? SymbolCategory.CONST : SymbolCategory.LET;
        var symbol = new Symbol(Identifier, Type, category, Location, null, Immutable, Async);
        Results.Add(context.SetSymbol(symbol));

        if(Assignment is IReaxResult result)
        {
            using(context.EnterFrom(Identifier))
            {
                Results.Add(result.Validate(context, Type));
                symbol.MarkAsAssigned();
            }
        }

        return ValidationResult.Join(Results);
    }
}
