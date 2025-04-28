using System;
using System.Text;
using Reax.Parser;
using Reax.Parser.Node.Interfaces;

namespace Reax.Semantic;

public class ValidationResult : IValidateResult
{   
    public ValidationResult(IEnumerable<IValidateResult> results)
    {
        StringBuilder builder = new StringBuilder();
        foreach (var result in results)
        {
            Status = Status && result.Status;
            if(!result.Status)
            {
                builder.Append(result.Source);
                builder.Append(" - ");
                builder.AppendLine(result.Message);
            }
        }

        Message = builder.ToString();
        Source = results.First().Source;
    }

    private ValidationResult(bool status, string message, SourceLocation source)
    {
        Status = status;
        Message = message;
        Source = source;
    }

    public bool Status { get; init; } =  true;

    public string Message { get; init; } = string.Empty;

    public SourceLocation Source { get; init; }

    public static IValidateResult Success(SourceLocation location) 
    {
        return new ValidationResult(true, "", location);        
    }

    public static IValidateResult ErrorAlreadyDeclared(string identifier, SourceLocation location) 
    {
        return new ValidationResult(false, $"O identificador {identifier} já foi declarado!", location);        
    }

    public static IValidateResult Join(IEnumerable<IValidateResult> results)
    {
        return new ValidationResult(results);
    }
}
