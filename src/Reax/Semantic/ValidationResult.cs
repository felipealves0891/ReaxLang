using System;
using System.Text;

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
        if(result._message.Length > 0)
        {
            if(_message.Length > 0)
                _message.AppendLine();

            _message.Append(result._message);
        }

        return this;
    }

    public static ValidationResult Success()
        => new ValidationResult(true);

    public static ValidationResult SymbolAlreadyDeclared(string identifier)
        => new ValidationResult(false, $"O simbolo {identifier} já foi declarado!");


}
