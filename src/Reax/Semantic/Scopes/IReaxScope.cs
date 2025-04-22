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

    void Declaration(IReaxDeclaration declaration);

    void Declaration(IReaxMultipleDeclaration declaration);

    Symbol Get(string identifier);

    void MarkAsAssigned(string identifier);

    void AddDependency(string from, string to);

    bool HasDependencyCycle();

    string GetPathDependencyCycle();
}
