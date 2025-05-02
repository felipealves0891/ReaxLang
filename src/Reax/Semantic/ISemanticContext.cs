using System;
using Reax.Semantic.Contexts;

namespace Reax.Semantic;

public interface ISemanticContext
{
    ValidationResult Declare(Symbol symbol);
    Symbol? Resolve(string identifier);
}
