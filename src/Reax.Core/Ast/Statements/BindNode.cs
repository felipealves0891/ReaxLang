using System;
using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;
using Reax.Core.Types;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Statements;
using Reax.Core.Debugger;


namespace Reax.Core.Ast.Statements;

[ExcludeFromCodeCoverage]
public record BindNode(
    string Identifier, 
    AssignmentNode Node,
    DataType Type, 
    SourceLocation Location) : StatementNode(Location), IReaxDeclaration
{
    public override IReaxNode[] Children => Node.Assigned is ContextNode context ? context.Block : [Node.Assigned];

    public override void Execute(IReaxExecutionContext context)
    {
        var interpreter = context.CreateInterpreter($"bind->{Identifier}", [Node.Assigned]);
        context.Declare(Identifier);
        context.SetBind(Identifier, interpreter);
    }

    public override string ToString()
    {
        return $"bind {Identifier}: {Type} -> {{...}}";
    }
}
