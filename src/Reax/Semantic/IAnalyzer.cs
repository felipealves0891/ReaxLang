using System;
using Reax.Semantic.Nodes;

namespace Reax.Semantic;

public interface IAnalyzer
{
    IValidationResult Analyze(IEnumerable<INode> Nodes, ISemanticContext context);
}
