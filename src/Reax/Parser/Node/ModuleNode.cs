using Reax.Runtime.Functions;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Symbols;

namespace Reax.Parser.Node;

public record ModuleNode(string identifier, Dictionary<string, Function> functions, 
    SourceLocation Location) : ReaxNode(Location), IReaxMultipleDeclaration
{
    public Symbol GetSymbol(Guid scope)
    {
        return new Symbol(
            identifier,
            SymbolType.NONE,
            SymbolCategoty.MODULE,
            scope
        );
    }

    public Symbol[] GetSymbols(Guid scope)
    {
        List<Symbol> symbols = new List<Symbol>();
        foreach (var key in functions.Keys)
        {
            var function = functions[key];
            if(function is IReaxMultipleDeclaration multipleDeclaration)
            {
                symbols.Add(multipleDeclaration.GetSymbol(scope));
                symbols.AddRange(multipleDeclaration.GetSymbols(scope));
            }
            else
            {
                symbols.Add(new Symbol(
                    key,
                    SymbolType.NONE,
                    SymbolCategoty.FUNCTION,
                    scope
                ));
            }
            
        }
        
        return symbols.ToArray();
    }

    public override string ToString()
    {
        return $"import module {identifier};";
    }
}
