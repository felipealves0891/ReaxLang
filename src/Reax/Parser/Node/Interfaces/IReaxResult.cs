using System;

namespace Reax.Parser.Node.Interfaces;

public interface IReaxResult
{
    IValidateResult Validate(ISemanticContext context);   
}
