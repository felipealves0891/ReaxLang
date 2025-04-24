using System;

namespace Reax.Semantic.Interfaces;

public interface IReaxFunctionCall
{
    string Identifier { get; }
    IReaxType[] Parameters { get; }
}
