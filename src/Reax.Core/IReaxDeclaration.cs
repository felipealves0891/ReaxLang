using System;
using Reax.Core.Ast;

namespace Reax.Core;

public interface IReaxDeclaration : IReaxStatement
{
    public void Initialize(IReaxExecutionContext context);
}
