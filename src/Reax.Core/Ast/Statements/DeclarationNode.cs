using System.Diagnostics.CodeAnalysis;
using Reax.Core.Ast.Expressions;
using Reax.Core.Locations;
using Reax.Core.Types;


namespace Reax.Core.Ast.Statements;

[ExcludeFromCodeCoverage]
public record DeclarationNode(
    string Identifier, 
    bool Immutable, 
    bool Async, 
    DataType Type,
    AssignmentNode? Assignment, 
    string? ComplexType,
    SourceLocation Location) : StatementNode(Location), IReaxDeclaration
{
    public override IReaxNode[] Children => Assignment is not null ? [Assignment] : [];

    public override void Execute(IReaxExecutionContext context)
    {
        if(!Immutable)
        {
            context.DeclareVariable(Identifier, Async);
            if(Assignment is not null && Assignment is not AssignmentNode)
                new AssignmentNode(new VarNode(Identifier, Type, Location), Assignment, Location).Execute(context);    
            else if(Assignment is not null && Assignment is AssignmentNode assignment)
                assignment.Execute(context);    
        }
        else 
        {
            if(Assignment is null)
                throw new InvalidOperationException("A constante deve ser definida na declaração!");

            if(Assignment is AssignmentNode assignment)
                context.DeclareImmutable(Identifier, assignment.Assigned.GetValue(context));
            else
                context.DeclareImmutable(Identifier, Assignment.GetValue(context));
        }
    }

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);

        writer.Write(Identifier);
        writer.Write(Immutable);
        writer.Write(Async);
        writer.Write((int)Type);
        if(Assignment is not null)
            Assignment.Serialize(writer);
        else
            writer.Write((byte)0); // Indicate no assignment
        writer.Write(ComplexType ?? string.Empty);
        base.Serialize(writer);
    }

    public override string ToString()
    {
        var asc = Async ? "async " : "";
        var mut = Immutable ? "const" : "let";
        if (Assignment is not null)
            return $"{asc}{mut} {Identifier}: {Type} = {Assignment};";
        else
            return $"{asc}{mut} {Identifier}: {Type};";
    }
}
