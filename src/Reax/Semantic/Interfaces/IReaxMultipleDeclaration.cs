using System;
using Reax.Semantic.Symbols;

namespace Reax.Semantic.Interfaces;

public interface IReaxMultipleDeclaration : IReaxDeclaration
{
    Symbol[] GetSymbols(Guid scope);
}
