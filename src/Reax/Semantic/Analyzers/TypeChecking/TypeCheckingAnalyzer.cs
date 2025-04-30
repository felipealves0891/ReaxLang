using System;
using Reax.Semantic.Node;
using Reax.Semantic.Results;

namespace Reax.Semantic.Analyzers.TypeChecking;

public class TypeCheckingAnalyzer : IAnalyzer
{
    private readonly IValidationResult result = new ValidationResult();
    
    public IValidationResult Analyze(IEnumerable<INode> nodes, ISemanticContext context)
    {
        foreach (var node in nodes)
        {
            if(node is INodeExpectedType expectedType)
            {
                using(context.ExpectedType(expectedType.ExpectedType))
                {
                    Analyze(expectedType.Children, context);
                }
            }
            else if (node is INodeResultType resultType)
            {
                if(context.ResultType(resultType.ResultType))
                    result.Join(new ValidationResult(true, ""));
                else
                    result.Join(new ValidationResult(false, $"{node.Location} - Erro de atribuição de tipos invalidos! Tipo atribuido '{resultType.ResultType}' tipo esperado '{context.CurrentType}'"));
            }
            else
            {
                Analyze(node.Children, context);
            }
        }

        return result;
    }
}
