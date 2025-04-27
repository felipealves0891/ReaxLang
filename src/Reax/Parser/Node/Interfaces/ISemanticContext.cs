using System;

namespace Reax.Parser.Node.Interfaces;

public interface IValidateResult
{
    bool Status { get; }
    string Message { get; }
    SourceLocation Source { get; }
}

public interface ISemanticContext
{}
