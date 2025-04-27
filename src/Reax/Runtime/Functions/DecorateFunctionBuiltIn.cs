using System;
using Reax.Parser.Node;
using Reax.Runtime.Functions.Attributes;
using Reax.Semantic.Interfaces;
using Reax.Semantic.Symbols;

namespace Reax.Runtime.Functions;

public class DecorateFunctionBuiltIn : Function, IReaxMultipleDeclaration
{
    private readonly FunctionBuiltInAttribute attribute;
    private readonly Function function;

    public DecorateFunctionBuiltIn(FunctionBuiltInAttribute attribute, Function function)
    {
        this.attribute = attribute;
        this.function = function;
    }

    public Symbol GetSymbol(Guid scope, string? module = null)
    {
        return new Symbol(
            attribute.Name,
            attribute.ReturnType,
            SymbolCategoty.FUNCTION,
            scope,
            parentName: attribute.Module
        );
    }

    public Symbol[] GetSymbols(Guid scope)
    {
        var length = attribute.ParameterTypes.Length;
        var symbols = new Symbol[length];
        for (int i = 0; i < length; i++)
        {
            symbols[i] = new Symbol(
                $"{attribute.Name}_parameter_{i}",
                attribute.ParameterTypes[i],
                attribute.MinParametersCount > i ? SymbolCategoty.PARAMETER : SymbolCategoty.PARAMETER_OPTIONAL,
                scope,
                parentName: attribute.Name
            );  
        }

        return symbols;
    }

    public override (ReaxNode? Success, ReaxNode? Error) Invoke(params ReaxNode[] parameters)
    {
        if(parameters.Length >= attribute.MinParametersCount && parameters.Length <= attribute.MaxParametersCount)
            return function.Invoke(parameters);
        
        throw new InvalidOperationException($"Função {attribute.Name} requer no minimo {attribute.MinParametersCount} parametros e no maximo {attribute.MaxParametersCount} e foi passado {parameters.Length}!");
    }
}
