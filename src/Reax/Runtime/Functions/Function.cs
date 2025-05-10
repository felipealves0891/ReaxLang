using System;
using Reax.Parser.Node;
using Reax.Parser.Node.Literals;

namespace Reax.Runtime.Functions;

public abstract class Function
{
    public abstract (LiteralNode? Success, LiteralNode? Error) Invoke(params ReaxNode[] parameters); 
}
