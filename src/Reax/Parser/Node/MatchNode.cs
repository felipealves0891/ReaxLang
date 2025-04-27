using System;
using Reax.Lexer;
using Reax.Parser.Node.Interfaces;

namespace Reax.Parser.Node;

public record MatchNode(
    ReaxNode Expression,    
    ActionNode Success,
    ActionNode Error,  
    SourceLocation Location) : ReaxNode(Location), IReaxResult
{
    public override string ToString()
    {
        return $"match {Expression} {{ success, error }}";
    }

    public IValidateResult Validate(ISemanticContext context)
    {
        throw new NotImplementedException();
    }
}
