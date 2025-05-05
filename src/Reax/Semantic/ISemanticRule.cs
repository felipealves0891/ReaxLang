using System;
using Reax.Parser.Node;

namespace Reax.Semantic;

public interface ISemanticRule
{
    ValidationResult Apply(IReaxNode node, ISemanticContext context);
    IDisposable? PrepareScope(ISemanticContext context);
}
