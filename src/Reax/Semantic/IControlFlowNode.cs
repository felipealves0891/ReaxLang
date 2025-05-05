using System;

namespace Reax.Semantic;

public interface IControlFlowNode
{
    bool HasGuaranteedReturn();
}
