using System.Collections.ObjectModel;
using Reax.Parser.Node;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Symbols;

namespace Reax.Semantic.Scopes;

public class ReaxScope : IReaxScope
{
    private readonly static Dictionary<Guid, Dictionary<string, Symbol>> _symbols 
        = new Dictionary<Guid, Dictionary<string, Symbol>>();

    public static IEnumerable<Symbol> Table => _symbols.Values.SelectMany(x => x.Values);

    private readonly Guid _scopeId = Guid.NewGuid();
    private readonly Dictionary<string, Symbol> _internal = new Dictionary<string, Symbol>();
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
    }

    public void Declaration(IReaxDeclaration declaration)
    {
        var symbol = declaration.GetSymbol(_scopeId);
        if(Exists(symbol.Identifier))
            throw new InvalidOperationException($"O simbulo {symbol.Identifier} já foi declarado!");
        
        _internal[symbol.Identifier] = symbol;
    }

    public void Declaration(IReaxMultipleDeclaration declaration)
    {
        foreach (var symbol in declaration.GetSymbols(_scopeId))
        {
            if(Exists(symbol.Identifier))
                throw new InvalidOperationException($"O simbulo {symbol.Identifier} já foi declarado!");
        
            _internal[symbol.Identifier] = symbol;    
        }
    }

    public Symbol Get(string identifier)
    {
        if(_internal.TryGetValue(identifier, out var local))
            return local;

        if(_parent is not null)
            return _parent.Get(identifier);
        else
            throw new InvalidOperationException($"O simbulo {identifier} não foi declarado!");
    }
    
}
