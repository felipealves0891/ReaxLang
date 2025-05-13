using System;
using Reax.Core.Ast;
using Reax.Core.Ast.Statements;
using Reax.Semantic.Contexts;

namespace Reax.Semantic.Rules;

public class ImmutableRule : BaseRule
{
    public ImmutableRule()
    {
        Handlers[typeof(AssignmentNode)] = ApplyAssignmentNode;
    }

    private ValidationResult ApplyAssignmentNode(IReaxNode node)
    {
        var assignment = (AssignmentNode)node;
        var symbol = Context.Resolve(assignment.Identifier.Identifier);
        if(symbol is null)
            return ValidationResult.FailureSymbolUndeclared(assignment.Identifier.Identifier, assignment.Location);
            
        if(symbol.Category is SymbolCategory.CONST or SymbolCategory.BIND)
        {
            if(symbol.Location.File != assignment.Location.File || symbol.Location.Start.Line != assignment.Location.Start.Line)
                return ValidationResult.FailureViolationOfImmutability(symbol.Identifier, symbol.Location);
        }
            

        return ValidationResult.Success();
    }
}
