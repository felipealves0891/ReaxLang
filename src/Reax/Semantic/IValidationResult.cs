using System;

namespace Reax.Semantic;

public interface IValidationResult
{
    bool IsValid { get; }
    string Message { get; }
    IValidationResult Join(IValidationResult result);
}
