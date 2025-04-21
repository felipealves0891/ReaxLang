using System;
using Reax.Semantic.Scopes;
using Reax.Semantic.Symbols;

namespace Reax.Semantic.Interfaces;

public interface IReaxType
{
    SymbolType GetReaxType(IReaxScope scope);
}
