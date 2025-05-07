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
        if(result._message.Length > 0 && !_message.ToString().Contains(result._message.ToString()))
        {
            if(_message.Length > 0)
                _message.AppendLine();

            _message.Append(result._message);
        }

        return this;
    }

    public static ValidationResult Success()
        => new ValidationResult(true);

    public static ValidationResult FailureSymbolAlreadyDeclared(string identifier, SourceLocation location)
        => new ValidationResult(false, $"{location} - O simbolo {identifier} já foi declarado!");

    public static ValidationResult FailureSymbolUndeclared(string identifier, SourceLocation location)
        => new ValidationResult(false, $"{location} - O simbolo {identifier} esta sendo usado, mas não foi declarado!");

    public static ValidationResult FailureIncompatibleTypes(DataType expected, DataType current, SourceLocation location)
        => new ValidationResult(false, $"{location} - Atribuição invalida! Era esperado {expected}, mas foi atribuido {current}!");
        
    public static ValidationResult FailureInvalidFunctionCall_ParametersCount(string identifier, int expected, int passed, SourceLocation location)
        => new ValidationResult(false, $"{location} - A função {identifier} esperava {expected}, mas foi passado {passed} parametros!");

    public static ValidationResult FailureInvalidFunctionCall_InvalidParameter(string identifier, string name, DataType expected, DataType passed, SourceLocation location)
        => new ValidationResult(false, $"{location} - O parametro {name} da função {identifier} esperava {expected}, mas foi passado {passed}!");

    public static ValidationResult FailureReactiveCycle(string identifier, SourceLocation location)
        => new ValidationResult(false, $"{location} - Ciclo reativo detectado envolvendo variável {identifier}");
    
    public static ValidationResult FailureControlFlow(SourceLocation location)
        => new ValidationResult(false, $"{location} - Nem todos os caminhos retornam valores!");
}
