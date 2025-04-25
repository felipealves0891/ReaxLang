using Reax.Semantic.Interfaces;
using Reax.Semantic.Symbols;

namespace Reax.Semantic.Scopes;

public class ReaxScope : IReaxScope
{
    private readonly static Dictionary<Guid, Dictionary<string, Symbol>> _symbols 
        = new Dictionary<Guid, Dictionary<string, Symbol>>();
    public static IEnumerable<Symbol> Table => _symbols.Values.SelectMany(x => x.Values);

    private readonly Guid _scopeId = Guid.NewGuid();
    private readonly Dictionary<string, List<Symbol>> _parameters = new Dictionary<string, List<Symbol>>();
    private readonly Dictionary<string, Symbol> _internal = new Dictionary<string, Symbol>();
    private readonly Dictionary<string, IReaxScope> _modules = new Dictionary<string, IReaxScope>();
    private readonly ReferenceVisitor _dependencies = new ReferenceVisitor();
    private readonly IReaxScope? _parent;

    public ReaxScope()
    {
        _symbols[_scopeId] = _internal;
    }

    public ReaxScope(IReaxScope parent)
        : this()
    {
        _parent = parent;
    }

    public Guid Id => _scopeId;

    public bool IsChild() => _parent is not null;

    public IReaxScope GetParent() => _parent 
        ?? throw new InvalidOperationException("Não é possivel recuperar o parent do escopo main!");

    public bool Exists(string identifier)
    {
        if(_internal.ContainsKey(identifier))
            return true;

        if(_parent is null)
            return false;
        else
            return _parent.Exists(identifier);
    }
    
    public void Declaration(Symbol symbol)
    {
        if(Exists(symbol.Identifier))
            throw new InvalidOperationException($"O simbulo {symbol.Identifier} já foi declarado!");
        
        _internal[symbol.Identifier] = symbol;

        if(!string.IsNullOrEmpty(symbol.ParentName) 
        && (symbol.Categoty == SymbolCategoty.PARAMETER || symbol.Categoty == SymbolCategoty.PARAMETER_OPTIONAL))
        {
            if(!_parameters.ContainsKey(symbol.ParentName))  _parameters[symbol.ParentName] = new List<Symbol>();
                 _parameters[symbol.ParentName].Add(symbol);
        }
            
    }

    public void Declaration(IReaxDeclaration declaration, string? module = null)
    {
        var symbol = declaration.GetSymbol(_scopeId, module);
        if(Exists(symbol.Identifier))
            throw new InvalidOperationException($"O simbulo {symbol.Identifier} já foi declarado!");
        
        _internal[symbol.Identifier] = symbol;
        
        if(!string.IsNullOrEmpty(symbol.ParentName) 
        && (symbol.Categoty == SymbolCategoty.PARAMETER || symbol.Categoty == SymbolCategoty.PARAMETER_OPTIONAL))
        {
            if(!_parameters.ContainsKey(symbol.ParentName))  _parameters[symbol.ParentName] = new List<Symbol>();
                 _parameters[symbol.ParentName].Add(symbol);
        }
    }

    public void Declaration(IReaxMultipleDeclaration declaration)
    {
        foreach (var symbol in declaration.GetSymbols(_scopeId))
        {
            if(Exists(symbol.Identifier))
                throw new InvalidOperationException($"O simbulo {symbol.Identifier} já foi declarado!");
        
            _internal[symbol.Identifier] = symbol;    
            if(!string.IsNullOrEmpty(symbol.ParentName) 
            && (symbol.Categoty == SymbolCategoty.PARAMETER || symbol.Categoty == SymbolCategoty.PARAMETER_OPTIONAL))
            {
                if(!_parameters.ContainsKey(symbol.ParentName))  _parameters[symbol.ParentName] = new List<Symbol>();
                    _parameters[symbol.ParentName].Add(symbol);
            }
        }
    }

    public Symbol Get(string identifier, string? module = null)
    {
        if(!string.IsNullOrEmpty(module))
        {
            if(_modules.TryGetValue(module, out var scope))
                return scope.Get(identifier);
        }
        else
        {
            if(_internal.TryGetValue(identifier, out var local))
                return local;
        }

        if(_parent is not null)
            return _parent.Get(identifier, module);
        else
            throw new InvalidOperationException($"O simbulo {identifier} não foi declarado!");
    }

    public void MarkAsAssigned(string identifier)
    {
        if(_internal.TryGetValue(identifier, out var symbol))
        {
            symbol.Assigned = true;
            _internal[identifier] = symbol;
        }
        else
            if(_parent is not null)
                _parent.MarkAsAssigned(identifier);
    }

    public void AddDependency(string from, string to)
    {
        if(_parent is not null)
        {
            _parent.AddDependency(from, to);
            return;
        }

        _dependencies.AddDependency(from, to);
    }

    public bool HasDependencyCycle()
    {
        return _dependencies.HasDependencyCycle();
    }

    public string GetPathDependencyCycle()
    {
        return string.Empty;
    }

    public Symbol[] GetParameters(string identifier, string? module = null)
    {
        if(!string.IsNullOrEmpty(module))
        {
            if(_modules.TryGetValue(module, out var scope))
                return scope.GetParameters(identifier);
        }
        else
        {
            if(_parameters.TryGetValue(identifier, out var symbols))
                return symbols.ToArray();
        }

        if(_parent is not null)
            return _parent.GetParameters(identifier, module);

        return [];
    }

    public void AddExtensionContext(string identifier, IReaxScope scope)
    {
        if(_modules.ContainsKey(identifier))
            throw new InvalidOperationException($"O modulo '{identifier}' já foi declarado!");

        _modules[identifier] = scope;
    }
}
