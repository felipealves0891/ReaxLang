using Reax.Core.Ast.Literals;
using Reax.Core.Ast;
using Reax.Core.Ast.Interfaces;

namespace Reax.Core.Functions;

public abstract class Function
{
    public abstract (IReaxValue? Success, IReaxValue? Error) Invoke(params IReaxValue[] parameters); 
}
