using System;
using Reax.Runtime.Contexts.Models;

namespace Reax.Runtime.Contexts;

public class ContextManager : IContext
{
    private readonly string _defaultModuleName;
    private readonly Module _modules;

    public ContextManager()
    {
        _defaultModuleName = Guid.NewGuid().ToString("N");
        _modules = new Module();
        _modules[_defaultModuleName] = new Stack<Scope>([new("main")]);
    }

    public bool CreateModule(string name)
    {
        if(_modules.ContainsKey(name)) return false;
        var scopes = new Stack<Scope>([new("main")]);
        return _modules.TryAdd(name, scopes);
    }

    public string CurrentScopeName(string? module = null)
    {
        var search = GetModuleName(module);
        return _modules[search].Peek().Name;
    }

    public bool Declare(Symbol symbol, string? module = null)
    {
        var search = GetModuleName(module);
        var scope = _modules[search].Peek();

        if(scope.ContainsKey(symbol.Identifier))
            return false;

        return scope.TryAdd(symbol.Identifier, symbol);
    }

    public IDisposable EnterScope(string name, string? module = null)
    {
        var search = GetModuleName(module);
        if(!_modules.ContainsKey(search)) _modules[search] = new Stack<Scope>();
        _modules[search].Push(new Scope(name));
        return new ExiterScope(() => ExitScope(search));
    }

    public string ExitScope(string? module = null)
    {
        var search = GetModuleName(module);
        return _modules[search].Pop().Name;
    }

    public Symbol GetSymbol(string identifier, string? module = null)
    {
        var search = GetModuleName(module);
        var currentscope = _modules[search].Peek();
        return currentscope[identifier];
    }

    public bool Update(Symbol symbol, string? module = null)
    {
        var search = GetModuleName(module);
        var currentscope = _modules[search].Peek();
        currentscope[symbol.Identifier] = symbol;
        return true;
    }

    private string GetModuleName(string? module = null)
    {
        return module ?? _defaultModuleName;
    }

    private class ExiterScope : IDisposable
    { 
        private readonly Action _action;
        private bool _disposed;

        public ExiterScope(Action action)
        {
            _action = action;
            _disposed = false;
        }

        public void Dispose()
        {
            if(_disposed)
                return;
            
            _action();
            _disposed = true;
        }
    }
}
