using Reax.Core.Types;
using Reax.Core.Ast.Literals;
using Reax.Core.Ast;
using Reax.Core.Functions.Attributes;

namespace Reax.Core.Functions;

public class DecorateFunctionBuiltIn : Function
{
    private readonly FunctionBuiltInAttribute attribute;
    private readonly Function function;

    public DecorateFunctionBuiltIn(FunctionBuiltInAttribute attribute, Function function)
    {
        this.attribute = attribute;
        this.function = function;
    }

    public override (LiteralNode? Success, LiteralNode? Error) Invoke(params ReaxNode[] parameters)
    {
        if(parameters.Length >= attribute.MinParametersCount && parameters.Length <= attribute.MaxParametersCount)
            return function.Invoke(parameters);
        
        throw new InvalidOperationException($"Função {attribute.Name} requer no minimo {attribute.MinParametersCount} parametros e no maximo {attribute.MaxParametersCount} e foi passado {parameters.Length}!");
    }

    public DataType Result => attribute.GetResult() ?? DataType.NONE;
    public DataType[] Parameters => attribute.GetParameters();
    public int ParametersCount => attribute.MaxParametersCount;
    public int RequiredParametersCount => attribute.MinParametersCount;

}
