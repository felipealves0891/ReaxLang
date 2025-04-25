using System;
using Reax.Semantic.Symbols;

namespace Reax.Semantic.Interfaces;

public interface IReaxBuiltIn : IReaxMultipleDeclaration
{
    string Identifier { get; }
}
