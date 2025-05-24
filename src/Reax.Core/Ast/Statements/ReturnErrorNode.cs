using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;
using Reax.Core.Ast.Literals;

namespace Reax.Core.Ast.Statements;

[ExcludeFromCodeCoverage]
public record ReturnErrorNode(
    ReaxNode Expression, 
    SourceLocation Location) : StatementNode(Location), IBranchFlowNode
{
    public override IReaxNode[] Children => [Expression];

    public override void Execute(IReaxExecutionContext context)
    {
        if (Expression is LiteralNode literal)
            throw new ReturnErrorException(literal);

        var interpreter = context.CreateInterpreter(ToString(), [Expression]);
        interpreter.Interpret();

        var output = interpreter.Output ?? throw new InvalidOperationException("Era esperado um retorno!");
        throw new ReturnErrorException(output);
    }

    public bool HasGuaranteedReturn()
    {
        return true;
    }

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);

        Expression.Serialize(writer);
        base.Serialize(writer);
    }

    public override string ToString()
    {
        return $"return error {Expression}";
    }
}
