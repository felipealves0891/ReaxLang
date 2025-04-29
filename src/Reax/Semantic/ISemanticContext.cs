using System;
using Reax.Parser;
using Reax.Semantic.Symbols;

namespace Reax.Semantic;

public interface ISemanticContext
{
    IDisposable EnterScope();
    void ExitScope();
    
    IDisposable EnterFrom(string name);
    void ExitFrom();
    void SetTo(string name);

    IValidationResult Declare(Symbol symbol);
    Symbol? Resolve(string identifier);

    IDisposable ExpectedType(DataType success, DataType error);
    bool ResultType(DataType dataType);
}
