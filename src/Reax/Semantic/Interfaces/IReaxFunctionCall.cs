using System;

namespace Reax.Semantic.Interfaces;

public interface IReaxFunctionCall
{
    string? Module { get; }
    string Identifier { get; }
    IReaxType[] Parameters { get; }
}
