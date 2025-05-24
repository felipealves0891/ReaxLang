using Reax.Core.Ast.Expressions;
using Reax.Core.Locations;
using Reax.Core.Types;

namespace Reax.Core.Ast.Statements;

public record ActionNode(
    VarNode Parameter,
    ReaxNode Context,
    DataType Type,
    SourceLocation Location) : StatementNode(Location), IBranchFlowNode
{
    public override IReaxNode[] Children => [Context];

    public override void Execute(IReaxExecutionContext context)
    {
    }

    public bool HasGuaranteedReturn()
    {
        if(Context is IBranchFlowNode control)
            return control.HasGuaranteedReturn();
        else
            return false;
    }

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);

        Parameter.Serialize(writer);
        Context.Serialize(writer);
        writer.Write((int)Type);
        base.Serialize(writer);
    }
    
    public override string ToString()
    {
        return $"({Parameter}){Type} -> {{...}}";
    }
}
