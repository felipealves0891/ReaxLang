using System;
using Reax.Semantic.Node;

namespace Reax.Semantic;

public interface IAnalyzer
{
    IValidationResult Analyze(IEnumerable<INode> Nodes, ISemanticContext context);
}
