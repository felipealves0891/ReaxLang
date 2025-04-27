using System;
using Reax.Parser;
using Reax.Parser.Node.Interfaces;

namespace Reax.Semantic;

public class ValidationResult : IValidateResult
{
    private ValidationResult(bool status, string message, SourceLocation source)
    {
        Status = status;
        Message = message;
        Source = source;
    }

    public bool Status { get; init; }

    public string Message { get; init; }

    public SourceLocation Source { get; init; }

    public static IValidateResult Success(SourceLocation location) 
    {
        return new ValidationResult(true, "", location);        
    }

    public static IValidateResult ErrorAlreadyDeclared(string identifier, SourceLocation location) 
    {
        return new ValidationResult(false, $"O identificador {identifier} já foi declarado!", location);        
    }
}
