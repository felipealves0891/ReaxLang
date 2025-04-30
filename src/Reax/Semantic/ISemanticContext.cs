using System;
using System.Collections.Concurrent;
using Reax.Parser;
using Reax.Semantic.Nodes;
using Reax.Semantic.Symbols;

namespace Reax.Semantic;

public interface ISemanticContext
{    
    ConcurrentDictionary<string, Symbol> CurrentScope { get; }
    string CurrentFrom { get; }
    MultiType CurrentType { get; }

    IDisposable EnterScope();
    void ExitScope();
    
    IDisposable EnterFrom(string name);
    void ExitFrom();
    void SetTo(string name);

    IValidationResult Declare(Symbol symbol);
    Symbol? Resolve(string identifier);

    IDisposable ExpectedType(MultiType type);
    bool ResultType(DataType dataType);
}
