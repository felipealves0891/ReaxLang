using System;
using Reax.Core.Ast;
using Reax.Parser.Node;

namespace Reax.Semantic;

public interface ISemanticAnalyzer
{
    ValidationResult Analyze(IReaxNode node, ISemanticContext context);
}
