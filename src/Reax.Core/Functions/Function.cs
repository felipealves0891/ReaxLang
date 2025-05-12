using Reax.Core.Ast.Literals;
using Reax.Core.Ast;

namespace Reax.Core.Functions;

public abstract class Function
{
    public abstract (LiteralNode? Success, LiteralNode? Error) Invoke(params ReaxNode[] parameters); 
}
