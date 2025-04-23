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

            if(node is IReaxBinder binder)
                ValidateBindCircularReferences(binder.Identifier, binder.Bound);

            if(node is IReaxObservable observable)
                ValidateObservableCircularReferences(observable.Identifier, (IReaxChildren)observable);

            if(node is IReaxContext reaxContext)
                Analyze(reaxContext.Context, _symbols, reaxContext);
        }    

        if(_symbols.IsChild())
            _symbols = _symbols.GetParent();
        else
            if(_symbols.HasDependencyCycle())
                throw new InvalidOperationException("Existe referencias cinculares!");
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

    private void ValidateBindCircularReferences(string identifier, IReaxChildren bound)
    {
        foreach (var child in bound.Children)
        {
            if(child is VarNode var)
                _symbols.AddDependency(identifier, var.Identifier);

            if(child is IReaxChildren children)
                ValidateBindCircularReferences(identifier, children);
        }
    }

    private void ValidateObservableCircularReferences(string identifier, IReaxChildren observable)
    {
        foreach (var child in observable.Children)
        {
            if(child is IReaxAssignment assignment)
                _symbols.AddDependency(identifier, assignment.Identifier);

            if(child is IReaxChildren children)
                ValidateObservableCircularReferences(identifier, children);
        }
    }
    
}
