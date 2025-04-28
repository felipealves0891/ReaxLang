using System;
using Reax.Parser;
using Reax.Parser.Node;
using Reax.Parser.Node.Interfaces;
using Reax.Runtime.Functions.Attributes;
using Reax.Semantic;
using Reax.Semantic.Symbols;

namespace Reax.Runtime.Functions;

public class DecorateFunctionBuiltIn : Function, IReaxResult
{
    private readonly FunctionBuiltInAttribute attribute;
    private readonly Function function;

    public DecorateFunctionBuiltIn(FunctionBuiltInAttribute attribute, Function function)
    {
        this.attribute = attribute;
        this.function = function;
    }

    public override (ReaxNode? Success, ReaxNode? Error) Invoke(params ReaxNode[] parameters)
    {
        if(parameters.Length >= attribute.MinParametersCount && parameters.Length <= attribute.MaxParametersCount)
            return function.Invoke(parameters);
        
        throw new InvalidOperationException($"Função {attribute.Name} requer no minimo {attribute.MinParametersCount} parametros e no maximo {attribute.MaxParametersCount} e foi passado {parameters.Length}!");
    }

    public IValidateResult Validate(ISemanticContext context, DataType expectedType = DataType.NONE)
    {
        var function = new Symbol(
            attribute.Name,
            Result,
            SymbolCategory.FUNCTION,
            new SourceLocation(),
            attribute.Module);

        var results = new List<IValidateResult>([context.SetSymbol(function)]);
        var parameters = Parameters;
        for (int i = 0; i < parameters.Count(); i++)
        {
            var category = SymbolCategory.PARAMETER;
            if(i >= attribute.MinParametersCount)
                category = SymbolCategory.PARAMETER_OPTIONAL;

            var symbol = new Symbol(
                $"{attribute.Name}_parameter_{i}",
                parameters[i],
                category,
                new SourceLocation(),
                attribute.Name
            );

            results.Add(context.SetSymbol(symbol));
        }

        return ValidationResult.Join(results);
    }

    public DataType Result => attribute.GetResult() ?? DataType.NONE;
    public DataType[] Parameters => attribute.GetParameters();
    public int ParametersCount => attribute.MaxParametersCount;
    public int RequiredParametersCount => attribute.MinParametersCount;

}
