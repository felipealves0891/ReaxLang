using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;
using Reax.Core.Helpers;


namespace Reax.Core.Ast.Statements;

[ExcludeFromCodeCoverage]
public record ReturnSuccessNode(
    ReaxNode Expression, 
    SourceLocation Location) : StatementNode(Location), IBranchFlowNode
{
    public override IReaxNode[] Children => [Expression];

    public override void Execute(IReaxExecutionContext context)
    {
        if (Expression is IReaxValue value)
            throw new ReturnSuccessException(value);
        
        var interpreter = context.CreateInterpreter(ToString(), [Expression]);
        interpreter.Interpret();

        var output = interpreter.Output ?? throw new InvalidOperationException("Era esperado um retorno!");
        throw new ReturnSuccessException(output);
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

    public static new ReturnSuccessNode Deserialize(BinaryReader reader)
    {
        var expression = BinaryDeserializerHelper.Deserialize<ReaxNode>(reader);
        var location = ReaxNode.Deserialize(reader);
        return new ReturnSuccessNode(expression, location);
    }

    public override string ToString()
    {
        return $"return success {Expression}";
    }
}
