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
                if(builder.Length > 0)
                    builder.AppendLine();

                builder.Append(result.Source);
                builder.Append(" - ");
                builder.Append(result.Message);
            }
        }

        Message = builder.ToString();
        Source = results.First().Source;
        TotalResults = results.Count();
    }

    private ValidationResult(bool status, string message, SourceLocation source)
    {
        Status = status;
        Message = message;
        Source = source;
        TotalResults = 1;
    }

    public bool Status { get; init; } =  true;

    public string Message { get; init; } = string.Empty;

    public SourceLocation Source { get; init; }

    public int TotalResults { get; init; }

    public static IValidateResult Success(SourceLocation location) 
    {
        return new ValidationResult(true, "", location);        
    }

    public static IValidateResult ErrorInvalidType(string identifier, SourceLocation location) 
    {
        return new ValidationResult(false, $"Atribuição de tipo incompativel no identificador {identifier}!", location);        
    }

    public static IValidateResult ErrorUndeclared(string identifier, SourceLocation location) 
    {
        return new ValidationResult(false, $"O identificador {identifier} não foi declarado, mas esta sendo usado!", location);        
    }

    public static IValidateResult ErrorAlreadyDeclared(string identifier, SourceLocation location) 
    {
        return new ValidationResult(false, $"O identificador {identifier} já foi declarado!", location);        
    }
    
    public static IValidateResult ErrorNoResultExpression(SourceLocation location) 
    {
        return new ValidationResult(false, $"Era esperado que a expressão retorna-se um resultado!", location);        
    }

    public static IValidateResult Join(IEnumerable<IValidateResult> results)
    {
        return new ValidationResult(results);
    }
}
