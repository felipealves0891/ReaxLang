using System;
using Reax.Debugger;
using Reax.Parser.Node;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Scopes;
using Reax.Semantic.Symbols;

namespace Reax.Semantic;

public class SemanticAnalyzer
{
    private IReaxScope _symbols;

    public SemanticAnalyzer()
    {
        _symbols = new ReaxScope();
    }

    public void Analyze(ReaxNode[] nodes, IReaxScope? parent = null, IReaxContext? context = null) 
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
            Logger.LogAnalize(node.ToString());

            if(node is IReaxExtensionContext extension)
                AddExtensionContext(extension);

            if(node is IReaxDeclaration declaration)
                _symbols.Declaration(declaration);

            if(node is IReaxMultipleDeclaration moduleDeclaration)
                _symbols.Declaration(moduleDeclaration);

            if(node is IReaxAssignment assignment)
                ValidateAssigment(assignment);

            if(node is IReaxContext reaxContext)
                Analyze(reaxContext.Context, _symbols, reaxContext);
        }    

        if(_symbols.IsChild())
            _symbols = _symbols.GetParent();
    }

    private void ValidateAssigment(IReaxAssignment assignment) 
    {
        var receivedSymbol = _symbols.Get(assignment.Identifier);
        if ((receivedSymbol.Immutable.HasValue && receivedSymbol.Immutable.Value) 
         && (receivedSymbol.Assigned.HasValue && receivedSymbol.Assigned.Value))
            throw new Exception($"{((ReaxNode)assignment).Location} - Tentativa de atribuir o valor a uma constante!");

        var assignedSymbol = assignment.TypeAssignedValue;
        var assignedType = assignedSymbol.GetReaxType(_symbols);

        if(!receivedSymbol.Type.IsCompatible(assignedType) && assignedSymbol is not NullNode)
            throw new Exception($"{((ReaxNode)assignment).Location} - Tentativa de atribuir o valor do tipo {assignedType} em uma variavel do tipo {receivedSymbol.Type}!");

        _symbols.MarkAsAssigned(assignment.Identifier);

    }

    private void AddExtensionContext(IReaxExtensionContext extensions)
    {
        IReaxScope current = _symbols;
        
        while(_symbols.IsChild())
            _symbols = _symbols.GetParent();
        
        Process(extensions.Context);

        _symbols = current;
    }
    
    private void Process(ReaxNode[] nodes)
    {
        foreach (var node in nodes)
        {
            if(node is IReaxDeclaration declaration)
                _symbols.Declaration(declaration);

            if(node is IReaxMultipleDeclaration moduleDeclaration)
                _symbols.Declaration(moduleDeclaration);

            if(node is IReaxAssignment assignment)
                ValidateAssigment(assignment);

            if(node is IReaxContext reaxContext)
                Analyze(reaxContext.Context, _symbols, reaxContext);
        }
    }
}
