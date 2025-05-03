using System;
using System.Collections;
using System.Collections.Concurrent;

namespace Reax.Semantic.Contexts;

public class SemanticContext : ISemanticContext
{
    private readonly ConcurrentStack<ConcurrentDictionary<string, Symbol>> _symbolsTable;
    private readonly ConcurrentStack<ConcurrentDictionary<string, HashSet<Symbol>>> _parametersTabe;
    
    public SemanticContext()
    {
        _symbolsTable = new();
        _parametersTabe = new();
        _symbolsTable.Push(new());
        _parametersTabe.Push(new());
    }

    public ConcurrentDictionary<string, Symbol> CurrentSymbolTable => 
        _symbolsTable.TryPeek(out var result) ? result : throw new Exception();

    public ConcurrentDictionary<string, HashSet<Symbol>> CurrentParametersTable => 
        _parametersTabe.TryPeek(out var result) ? result : throw new Exception();

    public ValidationResult Declare(Symbol symbol)
    {
        if(symbol.Category == SymbolCategory.PARAMETER)
        {
            if(symbol.ParentIdentifier is null)
                throw new InvalidOperationException("Todo simbolo da categoria de parametro deve conter o identificador do pai");

            if(!CurrentParametersTable.ContainsKey(symbol.ParentIdentifier)) 
                CurrentParametersTable[symbol.ParentIdentifier] = new HashSet<Symbol>();
            
            CurrentParametersTable[symbol.ParentIdentifier].Add(symbol);
            return ValidationResult.Success();
        }
        else 
        {
            if(CurrentSymbolTable.ContainsKey(symbol.Identifier))
                return ValidationResult.SymbolAlreadyDeclared(symbol.Identifier, symbol.Location);

            CurrentSymbolTable[symbol.Identifier] = symbol;
            return ValidationResult.Success();
        }
        
    }

    public IDisposable EnterScope()
    {
        _symbolsTable.Push(new());
        _parametersTabe.Push(new());
        return new ExiterScope(ExitScope);
    }

    public void ExitScope()
    {
        _parametersTabe.TryPop(out var _);
        _symbolsTable.TryPop(out var _);
    }

    public Symbol? Resolve(string identifier)
    {
        if(CurrentSymbolTable.TryGetValue(identifier, out var symbol))
            return symbol;

        foreach (var scope in _symbolsTable)
        {
            if(scope.TryGetValue(identifier, out var symbolOfParent))
                return symbolOfParent;
        }

        return null;
    }

    public Symbol[] ResolveParameters(string identifier)
    {
        if(CurrentParametersTable.ContainsKey(identifier))
            return CurrentParametersTable[identifier].ToArray();
        
        foreach (var table in _parametersTabe)
        {
            if(table.TryGetValue(identifier, out var symbolOfParent))
                return symbolOfParent.ToArray();
        }

        return [];

    }

    private class ExiterScope : IDisposable
    {
        private readonly Action _disposable;
        private bool _disposed = false;
        public ExiterScope(Action disposable) =>_disposable = disposable;
        public void Dispose()
        {
            if(!_disposed) _disposable();
            _disposed = true;
        }
    }
}
