using System;
using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;

namespace Reax.Core.Ast.Statements;

[ExcludeFromCodeCoverage]
public abstract record StatementNode(SourceLocation Location)
    : ReaxNode(Location), IReaxStatement
{
    public abstract void Execute(IReaxExecutionContext context);
}
