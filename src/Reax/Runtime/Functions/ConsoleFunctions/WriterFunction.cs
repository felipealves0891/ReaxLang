using Reax.Parser;
using Reax.Parser.Node;
using Reax.Runtime.Functions.Attributes;

namespace Reax.Runtime.Functions.ConsoleFunctions;

[FunctionBuiltIn(
    "console", 
    "writer", 
    1, 
    9, 
    TypeSource = typeof(WriterFunction), 
    ParametersProperty = nameof(Parameters),
    ResultProperty = nameof(Result))
]
public class WriterFunction : Function
{
    public override (ReaxNode? Success, ReaxNode? Error) Invoke(params ReaxNode[] parameters)
    {
        if(parameters.Length == 1)
            Console.WriteLine(parameters[0].ToString());
        else if(parameters.Length > 1 && parameters[0] is not null)
        {
            var format = parameters[0].ToString() ?? throw new InvalidOperationException("O formato é obrigadorio para o writer formatado!");
            Console.WriteLine(string.Format(format, parameters[1..]));
        }   

        return (null, null);
    }

    public static DataType[] Parameters => [
        DataType.STRING,
        DataType.STRING,
        DataType.STRING,
        DataType.STRING,
        DataType.STRING,
        DataType.STRING,
        DataType.STRING,
        DataType.STRING,
        DataType.STRING,
    ];

    public static DataType Result => DataType.VOID;
}
