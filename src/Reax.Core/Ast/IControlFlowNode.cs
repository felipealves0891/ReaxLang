using System;
using Reax.Core.Locations;
using Reax.Core.Types;
using Reax.Parser;

namespace Reax.Core.Ast;

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

