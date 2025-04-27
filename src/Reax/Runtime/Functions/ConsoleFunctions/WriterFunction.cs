using Reax.Parser.Node;
using Reax.Runtime.Functions.Attributes;

namespace Reax.Runtime.Functions.ConsoleFunctions;

[FunctionBuiltIn("console", "writer", 1, 10)]
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
}
