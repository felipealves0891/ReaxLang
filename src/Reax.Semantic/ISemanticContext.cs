using System;
using Reax.Semantic.Contexts;

namespace Reax.Semantic;

public interface ISemanticContext
{
    ValidationResult Declare(Symbol symbol);
    Symbol? Resolve(string identifier, string? script = null);
    Symbol[] ResolveChildren(string identifier, string? script = null);
    bool Remove(string identifier, string? script = null);
    
    IDisposable EnterScript(string name);
    void ExitScript();

    IDisposable EnterScope();
    void ExitScope();

    IDisposable EnterFrom(Reference from);
    void ExitFrom();
    void SetTo(Reference to);
    ValidationResult ValidateCycle();
}
