using System;
using Reax.Parser;

namespace Reax.Semantic;

public interface IBranchFlowNode
{
    bool HasGuaranteedReturn();
}

public interface IControlFlowNode : IBranchFlowNode
{
    DataType ResultSuccess { get; }
    DataType ResultError { get; }
    SourceLocation Location { get; }
}

