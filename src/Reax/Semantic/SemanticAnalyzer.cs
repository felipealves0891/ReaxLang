using System;
using Reax.Debugger;
using Reax.Parser.Node;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Scopes;
using Reax.Semantic.Symbols;

namespace Reax.Semantic;

public class SemanticAnalyzer
{
    private readonly string? _moduleName;
    private IReaxScope _symbols;
    
    public SemanticAnalyzer(string? module = null)
    {
        _symbols = new ReaxScope();
        _moduleName = module;
    }
    
    public IReaxScope Scope => _symbols;

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

        Process(nodes);
        
        if(_symbols.IsChild())
            _symbols = _symbols.GetParent();
        else
            if(_symbols.HasDependencyCycle())
                throw new InvalidOperationException("Existe referencias cinculares!");
    }

    private void Process(ReaxNode[] nodes)
    {
        foreach (var node in nodes)
        {
            Logger.LogAnalize(node.ToString());

            if(node is IReaxBuiltIn builtIn)
            {
                AddBuiltInContext(builtIn);
                continue;
            }
            
            if(node is IReaxExtensionContext extension)
            {
                AddExtensionContext(extension);
                continue;
            }
            
            if(node is IReaxDeclaration declaration) _symbols.Declaration(declaration, _moduleName);
            if(node is IReaxMultipleDeclaration moduleDeclaration) _symbols.Declaration(moduleDeclaration);
            if(node is IReaxAssignment assignment) ValidateAssigment(assignment);
            if(node is IReaxBinder binder) ValidateBindCircularReferences(binder.Identifier, binder.Bound);
            if(node is IReaxObservable observable) ValidateObservableCircularReferences(observable.Identifier, (IReaxChildren)observable);
            if(node is IReaxFunctionCall functionCall) ValidateFunctionCall(functionCall);
            if(node is IReaxContext reaxContext) Analyze(reaxContext.Context, _symbols, reaxContext);
        }
    }

    private void AddBuiltInContext(IReaxBuiltIn builtIn)
    {
        var context = new ReaxScope();
        context.Declaration((IReaxDeclaration)builtIn, builtIn.Identifier);
        context.Declaration((IReaxMultipleDeclaration)builtIn);
        _symbols.AddExtensionContext(builtIn.Identifier, context);
    }

    private void AddExtensionContext(IReaxExtensionContext extensions)
    {
        var analyzer = new SemanticAnalyzer(extensions.Identifier);
        analyzer.Process(extensions.Context);
        _symbols.AddExtensionContext(extensions.Identifier, analyzer.Scope);
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

    private void ValidateFunctionCall(IReaxFunctionCall functionCall)
    {
        var function = _symbols.Get(functionCall.Identifier, functionCall.Module);
        if(function.Categoty != SymbolCategoty.FUNCTION)
            throw new InvalidOperationException($"Chamada de função não localizada: {function.Identifier}");

        var declaredParameters = _symbols.GetParameters(functionCall.Identifier, functionCall.Module);
        var parameters = functionCall.Parameters;

        var requiredParameters = declaredParameters.Where(x => x.Categoty == SymbolCategoty.PARAMETER).Count();
        if(declaredParameters.Length < parameters.Length || requiredParameters > parameters.Length)
            throw new InvalidOperationException($"A função {function.Identifier} esperava {declaredParameters.Length} mas recebeu {parameters.Length}!");

        for (int i = 0; i < parameters.Length; i++)
        {
            var declared = declaredParameters[i];
            var argument = parameters[i].GetReaxType(_symbols);
            if(!declared.Type.IsCompatible(argument))
                throw new InvalidOperationException($"O parametro {declared.Identifier} da função {function.Identifier} deve ser do tipo {declared.Type}, mas foi passado {argument}!");
        }
    }
    
}
