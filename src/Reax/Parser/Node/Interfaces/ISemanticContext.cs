using System;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node.Interfaces;

public interface IValidateResult
{
    bool Status { get; }
    string Message { get; }
    SourceLocation Source { get; }
    int TotalResults { get; }
}

public interface ISemanticContext
{
    IValidateResult SetSymbol(Symbol symbol);
    Symbol? GetSymbol(string Identifier);
    Symbol[] GetParameters(string Identifier);
    
    IDisposable EnterScope();
    void ExitScope();

    IDisposable EnterFrom(string from);
    void ExitFrom();

    void SetDependency(string to);
}
