using System;
using System.Text;
using Reax.Parser;

namespace Reax.Semantic;

public class ValidationResult
{
    private bool _status;
    private StringBuilder _message;
    
    public bool Status => _status;
    public string Message => _message.ToString();

    private ValidationResult(bool status)
    {
        _status = status;
        _message = new StringBuilder();
    }
    
    private ValidationResult(bool status, string message)
    {
        _status = status;
        _message = new StringBuilder(message);
    }

    public ValidationResult Join(ValidationResult result) 
    {
        _status = _status && result.Status;
        if(result._message.Length > 0 && !MessageEquals(result._message, _message))
        {
            if(_message.Length > 0)
                _message.AppendLine();

            _message.Append(result._message);
        }

        return this;
    }

    private bool MessageEquals(StringBuilder sb1, StringBuilder sb2) 
    {
        if(sb1.Length != sb2.Length)
            return false;

        for (int i = 0; i < sb1.Length; i++)
        {
            if(sb1[i] != sb2[i])
                return false;
        }

        return true;
    }

    public static ValidationResult Success()
        => new ValidationResult(true);

    public static ValidationResult SymbolAlreadyDeclared(string identifier, SourceLocation location)
        => new ValidationResult(false, $"{location} - O simbolo {identifier} já foi declarado!");

    public static ValidationResult SymbolUndeclared(string identifier, SourceLocation location)
        => new ValidationResult(false, $"{location} - O simbolo {identifier} esta sendo usado, mas não foi declarado!");

    public static ValidationResult IncompatibleTypes(DataType expected, DataType current, SourceLocation location)
        => new ValidationResult(false, $"{location} - Atribuição invalida! Era esperado {expected}, mas foi atribuido {current}!");

}
