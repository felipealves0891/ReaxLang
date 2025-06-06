using System;
using System.Collections.Concurrent;
using Reax.Core.Types;
using Reax.Parser;
using Reax.Core.Ast.Statements;
using Reax.Core.Ast;

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
