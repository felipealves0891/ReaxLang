using System.Collections.Concurrent;
using Reax.Parser.Node.Interfaces;
using Reax.Semantic.Symbols;

namespace Reax.Semantic;

public class SemanticContext : ISemanticContext
{
    private readonly Stack<ConcurrentDictionary<string, Symbol>> _scopes;
    private readonly ConcurrentDictionary<string, List<string>> _referencies;
    private readonly Stack<string> _from;

    public SemanticContext()
    {
        _scopes = new();
        _referencies = new();
        _from = new();
    }

    public ConcurrentDictionary<string, Symbol> CurrentScope 
        => _scopes.Peek();

    public IDisposable EnterScope() 
    {
        _scopes.Push(new());
        return new Disposable(ExitScope);
    }

    public void ExitScope() 
    {
        _scopes.Pop();
    }
    
    public IDisposable EnterFrom(string from)
    {
        _from.Push(from);
        return new Disposable(ExitFrom);
    }

    public void ExitFrom()
    {
        _from.Pop();
    }

    public void SetDependency(string to)
    {
        var from = _from.Peek();
        if (!_referencies.ContainsKey(from)) _referencies[from] = new List<string>();
        _referencies[from].Add(to);
    }

    public IValidateResult SetSymbol(Symbol symbol)
    {
        if(CurrentScope.ContainsKey(symbol.Identifier))
            return ValidationResult.ErrorAlreadyDeclared(symbol.Identifier, symbol.Location);

        if(!CurrentScope.TryAdd(symbol.Identifier, symbol))
            return ValidationResult.ErrorAlreadyDeclared(symbol.Identifier, symbol.Location);

        return ValidationResult.Success(symbol.Location);
    }

    public Symbol? GetSymbol(string identifier)
    {
        if(CurrentScope.TryGetValue(identifier, out var symbol))
            return symbol;

        foreach (var scope in _scopes)
        {
            if(scope.TryGetValue(identifier, out var parentSymbol))
                return parentSymbol;
        }

        return null;
    }

    private sealed class Disposable : IDisposable
    {
        private readonly Action action;

        public Disposable(Action action)
        {
            this.action = action;
        }

        public void Dispose()
        {
            action();
        }
    }
}
