using System;
using Reax.Parser.Node;

namespace Reax.Semantic;

public interface ISemanticAnalyzer
{
    ValidationResult Analyze(IReaxNode node, ISemanticContext context);
}
