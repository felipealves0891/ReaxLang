using System;
using Reax.Parser;
using Reax.Parser.Node;
using Reax.Parser.Node.Interfaces;
using Reax.Runtime;

namespace Reax.Semantic;

public class SemanticAnalyzer
{
    private readonly SemanticTypeAnalyzer _typeAnalyzer;

    public SemanticAnalyzer()
    {
        _typeAnalyzer = new();
    }

    public void Analyze(IEnumerable<ReaxNode> nodes) 
    {
        _typeAnalyzer.TypeAnalyzer(nodes);
    }

}
