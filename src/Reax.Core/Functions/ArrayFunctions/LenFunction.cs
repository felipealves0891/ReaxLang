using System;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;
using Reax.Core.Ast.Objects;
using Reax.Core.Functions.Attributes;
using Reax.Core.Functions.ConsoleFunctions;
using Reax.Core.Locations;
using Reax.Core.Types;
using Reax.Parser.Node;

namespace Reax.Core.Functions.ArrayFunctions;

[FunctionBuiltIn(
    "array",
    "len",
    1,
    TypeSource = typeof(LenFunction),
    ParametersProperty = nameof(Parameters),
    ResultProperty = nameof(Result))
]
public class LenFunction : Function
{
    public override (IReaxValue? Success, IReaxValue? Error) Invoke(params IReaxValue[] parameters)
    {
        if (parameters.Length != 1)
            return (new NumberNode("-1", new SourceLocation()), null);

        var array = parameters[0] as ArrayNode;
        if (array is null)
            return (new NumberNode("-1", new SourceLocation()), null);

        return (new NumberNode(array.Literals.Length.ToString(), new SourceLocation()), null);
    }
    
    public static DataType[] Parameters => [DataType.ARRAY];
    public static DataType Result => DataType.NUMBER;
}
