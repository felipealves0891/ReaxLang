using System;
using System.Collections;
using System.Collections.Concurrent;

namespace Reax.Semantic.Contexts;

public class SemanticContext : ISemanticContext
{
    private readonly ConcurrentStack<ConcurrentDictionary<string, Symbol>> _symbolsTable;
    private readonly ConcurrentStack<ConcurrentDictionary<string, HashSet<Symbol>>> _parametersTabe;
    private readonly ConcurrentStack<string> _scripts;

    public SemanticContext()
    {
        _symbolsTable = new();
        _parametersTabe = new();
        _scripts = new();

        _symbolsTable.Push(new());
        _parametersTabe.Push(new());
        _scripts.Push("main");
        
    }

    public ConcurrentDictionary<string, Symbol> CurrentSymbolTable => 
        _symbolsTable.TryPeek(out var result) ? result : throw new Exception();

    public ConcurrentDictionary<string, HashSet<Symbol>> CurrentParametersTable => 
        _parametersTabe.TryPeek(out var result) ? result : throw new Exception();

    public string CurrentScript => 
        _scripts.TryPeek(out var name) ? name : string.Empty;

    public ValidationResult Declare(Symbol symbol)
    {
        var identifier = GetIdentifier(symbol.Identifier);

        if(symbol.Category == SymbolCategory.PARAMETER)
            return DeclareParameters(symbol);
        
        if(CurrentSymbolTable.ContainsKey(identifier))
            return ValidationResult.SymbolAlreadyDeclared(identifier, symbol.Location);

        CurrentSymbolTable[identifier] = symbol;
        return ValidationResult.Success();    
    }

    private ValidationResult DeclareParameters(Symbol symbol)
    {
        if(symbol.ParentIdentifier is null)
                throw new InvalidOperationException("Todo simbolo da categoria de parametro deve conter o identificador do pai");

        var identifier = GetIdentifier(symbol.ParentIdentifier);

        if(!CurrentParametersTable.ContainsKey(identifier)) 
            CurrentParametersTable[identifier] = new HashSet<Symbol>();
        
        var isAlreadyDeclared =  CurrentParametersTable[identifier].Any(x => x.Identifier == symbol.Identifier);
        if(isAlreadyDeclared)
            return ValidationResult.SymbolAlreadyDeclared(symbol.Identifier, symbol.Location);

        CurrentParametersTable[identifier].Add(symbol);
        return ValidationResult.Success();
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

    public Symbol? Resolve(string identifier, string? script = null)
    {
        var internalIdentifier = GetIdentifier(identifier, script);
        if(CurrentSymbolTable.TryGetValue(internalIdentifier, out var symbol))
            return symbol;

        foreach (var scope in _symbolsTable)
        {
            if(scope.TryGetValue(internalIdentifier, out var symbolOfParent))
                return symbolOfParent;
        }

        return null;
    }

    public Symbol[] ResolveParameters(string identifier, string? script = null)
    {
        var internalIdentifier = GetIdentifier(identifier, script);
        if(CurrentParametersTable.ContainsKey(internalIdentifier))
            return CurrentParametersTable[internalIdentifier].ToArray();
        
        foreach (var table in _parametersTabe)
        {
            if(table.TryGetValue(internalIdentifier, out var symbolOfParent))
                return symbolOfParent.ToArray();
        }

        return [];

    }

    public IDisposable EnterScript(string name)
    {
        _scripts.Push(name);
        return new ExiterScope(ExitScript);
    }

    public void ExitScript()
    {
        _scripts.TryPop(out var _);
    }

    private string GetIdentifier(string identifier, string? script = null) 
    {
        if(string.IsNullOrEmpty(script))
            script = CurrentScript;

        return $"{script}.{identifier}";
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
