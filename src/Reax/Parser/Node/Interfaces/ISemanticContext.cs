using System;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node.Interfaces;

public interface IValidateResult
{
    bool Status { get; }
    string Message { get; }
    SourceLocation Source { get; }
}

public interface ISemanticContext
{
    IValidateResult SetSymbol(Symbol symbol);
    Symbol? GetSymbol(string Identifier);
    
    void EnterScope();
    void ExitScope();

    void EnterFrom(string from);
    void ExitFrom();

    void SetDependency(string to);
}
