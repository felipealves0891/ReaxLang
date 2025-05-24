using Reax.Core.Locations;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Statements;

namespace Reax.Core.Ast.Statements;

public record ContextNode(
    ReaxNode[] Block, 
    SourceLocation Location) : StatementNode(Location), IBranchFlowNode
{
    public override IReaxNode[] Children => Block;

    public override void Execute(IReaxExecutionContext context)
    {
        var interpreter = context.CreateInterpreter(ToString(), Block);
        interpreter.Interpret();
    }

    public bool HasGuaranteedReturn()
    {
        return Block.Any(x => x is IBranchFlowNode control ? control.HasGuaranteedReturn() : false);
    }

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);

        writer.Write(Block.Length);
        foreach (var node in Block)
        {
            node.Serialize(writer);
        }
        base.Serialize(writer);
    }

    public override string ToString()
    {
        return "{...}";
    }
}
