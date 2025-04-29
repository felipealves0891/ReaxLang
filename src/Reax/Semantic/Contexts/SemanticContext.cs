using System;
using Reax.Parser;
using Reax.Semantic.Results;
using Reax.Semantic.Symbols;

namespace Reax.Semantic.Contexts;

public class SemanticContext : ISemanticContext
{
    private readonly Stack<Dictionary<string, Symbol>> _scopes;
    private readonly Dictionary<string, IList<string>> _dependencies;
    private readonly Stack<string> _from;
    private readonly Stack<(DataType Success, DataType Error)> _expectedTypes;

    public SemanticContext()
    {
        _scopes = new();
        _dependencies = new();
        _from = new();
        _expectedTypes = new();
    }

    public Dictionary<string, Symbol> CurrentScope => _scopes.Peek();
    public string CurrentFrom => _from.Peek();
    public (DataType Success, DataType Error) CurrentType => _expectedTypes.Peek();

    public IValidationResult Declare(Symbol symbol)
    {
        if(CurrentScope.TryGetValue(symbol.Identifier, out var declaredSymbol))
            return new ValidationResult(false, $"Simbolo '{symbol.Identifier}' já foi declarado em {declaredSymbol.Location}");

        if(CurrentScope.TryAdd(symbol.Identifier, symbol))
            return new ValidationResult(true, "");
        else
            return new ValidationResult(false, $"Simbolo '{symbol.Identifier}' não pode ser declarado!");
    }

    public IDisposable EnterFrom(string name)
    {
        _from.Push(name);
        return new ExitDisposable(ExitFrom);
    }

    public IDisposable EnterScope()
    {
        _scopes.Push(new());
        return new ExitDisposable(ExitScope);
    }

    public void ExitFrom()
    {
        _from.Pop();
    }

    public void ExitScope()
    {
        _scopes.Pop();
    }

    public IDisposable ExpectedType(DataType success, DataType error)
    {
        _expectedTypes.Push((success, error));
        return new ExitDisposable(() => _expectedTypes.Pop());
    }

    public Symbol? Resolve(string identifier)
    {
        if(CurrentScope.TryGetValue(identifier, out var currentSymbol))
            return currentSymbol;

        foreach (var scope in _scopes)
        {
            if(scope.TryGetValue(identifier, out var parentSymbol))
                return parentSymbol;
        }

        return null;
    }

    public bool ResultType(DataType type)
    {
        return CurrentType.Success == type
            || CurrentType.Error == type;
        
    }

    public void SetTo(string name)
    {
        if(!_dependencies.ContainsKey(CurrentFrom)) _dependencies[CurrentFrom] = new List<string>();
        _dependencies[CurrentFrom].Add(name);
    }

    private class ExitDisposable : IDisposable
    {
        private readonly Action action;

        public ExitDisposable(Action action)
        {
            this.action = action;
        }

        public void Dispose()
        {
            action();
        }
    }
}
