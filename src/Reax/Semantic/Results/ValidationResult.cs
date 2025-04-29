using System;
using System.Text;

namespace Reax.Semantic.Results;

public class ValidationResult : IValidationResult
{
    private List<IValidationResult> _results;
    private bool _isValid = false;
    private StringBuilder _message = new StringBuilder();

    public ValidationResult()
    {
        _results = new List<IValidationResult>();
    }
    
    public ValidationResult(bool isValid, string message)
        : this()
    {
        _isValid = isValid;
        _message.Append(message);
    }

    public bool IsValid => _isValid;
    public string Message => _message.ToString();

    public IValidationResult Join(IValidationResult result)
    {
        _results.Add(result);
        _isValid = _isValid && result.IsValid;

        if(_message.Length > 0)
            _message.AppendLine();

        _message.Append(result);
        return this;
    }
}
