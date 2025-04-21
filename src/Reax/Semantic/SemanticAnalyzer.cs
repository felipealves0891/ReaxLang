using System;
using Reax.Parser.Node;
using Reax.Parser.Node.Interfaces;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Scopes;
using Reax.Semantic.Symbols;

namespace Reax.Semantic;

public class SemanticAnalyzer
{
    private ReaxScope _symbols;

    public SemanticAnalyzer()
    {
        _symbols = new ReaxScope();
    }

    public void Analyze(ReaxNode[] nodes, ReaxScope? parent = null, IReaxContext? context = null) 
    {
        if(parent is not null)
            _symbols = new ReaxScope(parent);
        
        if(context is not null)
        {
            var parameters = context.GetParameters(_symbols.Id);
            foreach (var parameter in parameters)
                _symbols.Declaration(parameter);
        }   

        foreach (var node in nodes)
        {
            if(node is IReaxDeclaration declaration)
                _symbols.Declaration(declaration);

            if(node is IReaxMultipleDeclaration moduleDeclaration)
                _symbols.Declaration(moduleDeclaration);

            if(node is AssignmentNode assignment)
                ValidateAssignment(assignment);

            if(node is IReaxContext reaxContext)
                Analyze(reaxContext.Context, _symbols, reaxContext);
        }    

        _symbols = _symbols.GetParent();
    }

    private void ValidateAssignment(AssignmentNode assignment) 
    {
        var symbol = _symbols.Get(assignment.Identifier);

        

    }
}
