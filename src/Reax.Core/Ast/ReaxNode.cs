using Reax.Core.Locations;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;

namespace Reax.Core.Ast;

public abstract record ReaxNode(SourceLocation Location) : IReaxNode
{
    public abstract IReaxNode[] Children { get; }
}
