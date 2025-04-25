using System;
using Reax.Semantic.Symbols;

namespace Reax.Semantic.Interfaces;

public interface IReaxDeclaration
{
    Symbol GetSymbol(Guid scope, string? module = null);
}
