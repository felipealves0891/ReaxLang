using Reax.Parser.Node.Statements;

namespace Reax.Semantic.Rules;

public class SymbolRule : BaseRule
{
    public SymbolRule() : base()        
    {
        Handlers[typeof(DeclarationNode)] = ApplyDeclarationNode;
    }

    private ValidationResult ApplyDeclarationNode(IReaxNode node)
    {
        return ValidationResult.Success();
    }
    
}
