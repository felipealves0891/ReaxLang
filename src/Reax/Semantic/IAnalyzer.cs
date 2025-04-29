using System;
using Reax.Semantic.Node;

namespace Reax.Semantic;

public interface IAnalyzer
{
    IValidationResult Analyze(INode[] Nodes, ISemanticContext context);
}
