using System;
using System.Collections.Concurrent;
using Reax.Parser;
using Reax.Parser.Node.Statements;

namespace Reax.Semantic.Rules;

public class ReturnFlowRule : ISemanticRule
{

    public ValidationResult Apply(IReaxNode node, ISemanticContext context)
    {
        if(node is IControlFlowNode controlFlow)
        {
            var isVoid = controlFlow.ResultSuccess == DataType.VOID && controlFlow.ResultError == DataType.VOID;
            if(!controlFlow.HasGuaranteedReturn() && !isVoid)
                return ValidationResult.FailureControlFlow(node.Location);    
        }

        return ValidationResult.Success();
    }

    public IDisposable? PrepareScope(ISemanticContext context)
        => null!;
}
