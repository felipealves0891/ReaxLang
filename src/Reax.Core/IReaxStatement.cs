using System;

namespace Reax.Core;

public interface IReaxStatement
{
    void Accept(IReaxInterpreter interpreter);
}
