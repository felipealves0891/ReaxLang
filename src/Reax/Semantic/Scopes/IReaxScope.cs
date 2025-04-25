using System;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Symbols;

namespace Reax.Semantic.Scopes;

public interface IReaxScope
{
    Guid Id { get; }

    bool IsChild();

    IReaxScope GetParent();

    bool Exists(string identifier);

    void Declaration(Symbol symbol);

    void Declaration(IReaxDeclaration declaration, string? module = null);

    void Declaration(IReaxMultipleDeclaration declaration);

    Symbol Get(string identifier, string? module = null);

    Symbol[] GetParameters(string identifier, string? module = null);

    void MarkAsAssigned(string identifier);

    void AddDependency(string from, string to);

    bool HasDependencyCycle();

    string GetPathDependencyCycle();

    void AddExtensionContext(string identifier, IReaxScope scope);
}
