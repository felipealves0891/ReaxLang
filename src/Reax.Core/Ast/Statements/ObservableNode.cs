using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Interfaces;


namespace Reax.Core.Ast.Statements;

[ExcludeFromCodeCoverage]
public record ObservableNode(
    VarNode Var, 
    ContextNode Block, 
    BinaryNode? Condition, 
    SourceLocation Location) : StatementNode(Location)
{
    public override IReaxNode[] Children => Condition is null ? [Var, Block] : [Var, Block, Condition];

    public override void Execute(IReaxExecutionContext context)
    {
        var identifier = Var.Identifier;
        var interpreter = context.CreateInterpreter(ToString(), Block.Block);
        context.SetObservable(identifier, interpreter, Condition);
    }

    public override string ToString()
    {
        var when = Condition is null ? "" : $"when {Condition} "; 
        return $"on {Var} {when} {{...}}";
    }
}
