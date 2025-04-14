using System;
using Reax.Parser.Node;

namespace Reax.Runtime.Functions;

public abstract class Function
{
    public abstract ReaxNode? Invoke(params ReaxNode[] parameters); 
}
