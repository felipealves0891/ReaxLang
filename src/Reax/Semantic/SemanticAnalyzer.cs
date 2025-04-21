using System;
using Reax.Parser.Node;
using Reax.Semantic.Symbols;

namespace Reax.Semantic;

public class SemanticAnalyzer
{
    private readonly IDictionary<string, IList<Symbol>> _symbols;

    public SemanticAnalyzer()
    {
        _symbols = new Dictionary<string, IList<Symbol>>();

    }

    public void Analyze(ReaxNode[] nodes) 
    {
        
    }
}
