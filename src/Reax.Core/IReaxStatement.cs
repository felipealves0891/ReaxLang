using System;

namespace Reax.Core;

public interface IReaxStatement
{
    void Execute(IReaxExecutionContext context);
}
