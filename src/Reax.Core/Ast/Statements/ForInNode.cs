using System;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Objects;
using Reax.Core.Locations;

namespace Reax.Core.Ast.Statements;

public record ForInNode(
    DeclarationNode Declaration,
    ReaxNode Array,
    ContextNode Context,
    SourceLocation Location) : StatementNode(Location)
{
    public override IReaxNode[] Children => [Declaration, Array, Context];

    public override void Execute(IReaxExecutionContext context)
    {
        Declaration.Execute(context);
        var array = GetArray(context);

        foreach (var item in array)
        {
            context.SetVariable(Declaration.Identifier, item.GetValue(context));
            Context.Execute(context);
        }
    }

    private ArrayNode GetArray(IReaxExecutionContext context)
    {
        if (Array is VarNode var)
            return (ArrayNode)context.GetVariable(var.Identifier);
        else
            return (ArrayNode)Array;
    }
}
