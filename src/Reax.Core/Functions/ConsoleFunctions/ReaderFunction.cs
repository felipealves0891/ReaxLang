using System;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;
using Reax.Core.Functions.Attributes;
using Reax.Core.Locations;
using Reax.Core.Types;

namespace Reax.Core.Functions.ConsoleFunctions;

[FunctionBuiltIn(
    "console", 
    "reader", 
    0, 
    0, 
    TypeSource = typeof(ReaderFunction), 
    ParametersProperty = nameof(Parameters),
    ResultProperty = nameof(Result))
]
public class ReaderFunction : Function
{
    public override (IReaxValue? Success, IReaxValue? Error) Invoke(params IReaxValue[] parameters)
    {
        var input = Console.ReadLine() ?? "";
        return (new StringNode(input, new SourceLocation()), null);
    }
    
    public static DataType[] Parameters => [];
    public static DataType Result => DataType.STRING;

}
