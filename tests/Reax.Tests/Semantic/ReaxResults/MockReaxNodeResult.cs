using System;
using Reax.Parser;
using Reax.Parser.Node;
using Reax.Parser.Node.Interfaces;

namespace Reax.Tests.Semantic.ReaxResults;

public record MockReaxNodeResult(Func<ISemanticContext, DataType, IValidateResult> ActionValidate) : ReaxNode(new SourceLocation()), IReaxResult
{
    public IValidateResult Validate(ISemanticContext context, DataType expectedType = DataType.NONE)
    {
        return ActionValidate(context, expectedType);
    }
}
