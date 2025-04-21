using System;
using Reax.Parser.Node;
using Reax.Parser.Node.Interfaces;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Scopes;
using Reax.Semantic.Symbols;

namespace Reax.Semantic;

public class SemanticAnalyzer
{
    private readonly ReaxScope _symbols;

    public SemanticAnalyzer()
    {
        _symbols = new ReaxScope();
    }

    public SemanticAnalyzer(ReaxScope parent)
    {
        _symbols = new ReaxScope(parent);
    }

    public void Analyze(ReaxNode[] nodes) 
    {
        foreach (var node in nodes)
        {
            if(node is IReaxDeclaration declaration)
                _symbols.Declaration(declaration);

            if(node is IReaxModuleDeclaration moduleDeclaration)
                _symbols.Declaration(moduleDeclaration);

            if(node is IReaxContext context)
                Analyze(context.Context);
        }    
    }
}
