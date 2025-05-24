using System;
using System.Collections;
using System.Collections.Concurrent;
using Reax.Core.Debugger;
using Reax.Core.Ast.Statements;

namespace Reax.Semantic.Contexts;

public class SemanticContext : ISemanticContext
{
    private readonly ConcurrentStack<ConcurrentDictionary<string, Symbol>> _symbolsTable;
    private readonly ConcurrentStack<ConcurrentDictionary<string, HashSet<Symbol>>> _childrenTabe;
    private readonly ConcurrentDictionary<Reference, HashSet<Reference>> _references;
    private readonly ValidationResult _validationReferences;
    private readonly ConcurrentStack<Reference> _from;
    private readonly ConcurrentStack<string> _scripts;

    public SemanticContext()
    {
        _symbolsTable = new();
        _childrenTabe = new();
        _scripts = new();
        _from = new();
        _references = new();
        _validationReferences = ValidationResult.Success();

        _symbolsTable.Push(new());
        _childrenTabe.Push(new());
        _scripts.Push("main");        
    }

    public ConcurrentDictionary<string, Symbol> CurrentSymbolTable => 
        _symbolsTable.TryPeek(out var result) ? result : throw new Exception();

    public ConcurrentDictionary<string, HashSet<Symbol>> CurrentChildrenTable => 
        _childrenTabe.TryPeek(out var result) ? result : throw new Exception();

    public string CurrentScript => 
        _scripts.TryPeek(out var name) ? name : string.Empty;

    public Reference? CurrentFrom => 
        _from.TryPeek(out var name) ? name : null!;

    public ValidationResult Declare(Symbol symbol)
    {
        var identifier = GetIdentifier(symbol.Identifier);

        if(symbol.Category is SymbolCategory.PARAMETER or SymbolCategory.PARAMETER_OPTIONAL or SymbolCategory.PROPERTY)
            return DeclareChild(symbol);
        
        if(CurrentSymbolTable.ContainsKey(identifier))
            return ValidationResult.FailureSymbolAlreadyDeclared(identifier, symbol.Location);

        CurrentSymbolTable[identifier] = symbol;
        return ValidationResult.Success();    
    }

    private ValidationResult DeclareChild(Symbol symbol)
    {
        if(symbol.ParentIdentifier is null)
                throw new InvalidOperationException($"Todo simbolo da categoria de {symbol.Category} deve conter o identificador do pai");

        var identifier = GetIdentifier(symbol.ParentIdentifier);

        if(!CurrentChildrenTable.ContainsKey(identifier)) 
            CurrentChildrenTable[identifier] = new HashSet<Symbol>();
        
        var isAlreadyDeclared =  CurrentChildrenTable[identifier].Any(x => x.Identifier == symbol.Identifier);
        if(isAlreadyDeclared)
            return ValidationResult.FailureSymbolAlreadyDeclared(symbol.Identifier, symbol.Location);

        CurrentChildrenTable[identifier].Add(symbol);
        return ValidationResult.Success();
    }

    public IDisposable EnterScope()
    {
        _symbolsTable.Push(new());
        _childrenTabe.Push(new());
        return new ExiterScope(ExitScope);
    }

    public void ExitScope()
    {
        _childrenTabe.TryPop(out var _);
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

    public Symbol[] ResolveChildren(string identifier, string? script = null)
    {
        var internalIdentifier = GetIdentifier(identifier, script);
        if(CurrentChildrenTable.ContainsKey(internalIdentifier))
            return CurrentChildrenTable[internalIdentifier].ToArray();
        
        foreach (var table in _childrenTabe)
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

    public IDisposable EnterFrom(Reference from)
    {
        Logger.LogSemanticContext($"({from.Location.Start.Line}) -> EnterFrom('{from}')");
        _from.Push(from);
        return new ExiterScope(ExitFrom);
    }

    public void ExitFrom()
    {
        if(!_from.TryPop(out var variable))
            return;

        var visited = new HashSet<Reference>(new ReferenceEqualityComparer());
        var stack = new HashSet<Reference>(new ReferenceEqualityComparer());
        var results = ValidationResult.Success();

        Logger.LogSemanticContext($"({variable.Location.Start.Line}) -> ExitFrom({variable})");
        var hasCycle = HasCycle(variable, visited, stack);
        if (hasCycle.Status)
        {
            var cycleStr = string.Join(" → ", hasCycle.CyclePath);
            results.Join(ValidationResult.FailureReactiveCycle(cycleStr, variable.Location));
        } 

        _validationReferences.Join(results);
        _references.Clear();        
    }

    public void SetTo(Reference to)
    {
        if(CurrentFrom is null)
            return;
            
        if(!_references.ContainsKey(CurrentFrom)) _references[CurrentFrom] = new HashSet<Reference>();
        _references[CurrentFrom].Add(to);
        Logger.LogSemanticContext($"({to.Location.Start.Line}) -> SetTo('{to}')");
    }

    public ValidationResult ValidateCycle()
        => _validationReferences;

    private (bool Status, string[] CyclePath) HasCycle(Reference variable, HashSet<Reference> visited, HashSet<Reference> stack)
    {
        if (stack.Contains(variable)) 
        {
            var from = stack.First(x => x.Equals(variable));
            if(from.Type == typeof(ObservableNode))
            {
                if(!variable.IsAssignment)
                    return (true, GetCyclePath(variable, stack));
            }
            else 
                return (true, GetCyclePath(variable, stack));
        }

        if (visited.Contains(variable)) return (false, []);

        visited.Add(variable);
        stack.Add(variable);

        if (_references.TryGetValue(variable, out var deps))
        {
            foreach (var dep in deps)
            {
                var (status, cyclePath) = HasCycle(dep, visited, stack);
                if (status)
                    return (true, cyclePath);
            }
        }

        stack.Remove(variable);
        return (false, []);
    }

    private string[] GetCyclePath(Reference variable, HashSet<Reference> stack)
    {
        return stack.Reverse()
                    .Append(variable)
                    .Select(x => x.ToString())
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToArray();
    }

    public bool Remove(string identifier, string? script = null)
    {
        var internalIdentifier = GetIdentifier(identifier, script);
        var removed = false;

        if (CurrentSymbolTable.ContainsKey(internalIdentifier))
            removed = CurrentSymbolTable.TryRemove(internalIdentifier, out _);

        foreach (var scope in _symbolsTable)
        {
            if (scope.ContainsKey(internalIdentifier))
                removed = removed || scope.TryRemove(internalIdentifier, out _);
        }

        return removed;
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
