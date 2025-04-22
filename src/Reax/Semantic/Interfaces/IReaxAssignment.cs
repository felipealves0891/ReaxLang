using System;

namespace Reax.Semantic.Interfaces;

public interface IReaxAssignment
{
    string Identifier { get; }
    IReaxType TypeAssignedValue { get; }
}
