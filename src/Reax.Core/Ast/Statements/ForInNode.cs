using System;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Objects;
using Reax.Core.Ast.Objects.Structs;
using Reax.Core.Helpers;
using Reax.Core.Locations;

namespace Reax.Core.Ast.Statements;

public record ForInNode(
    DeclarationNode Declaration,
    ReaxNode Array,
    ContextNode Context,
    SourceLocation Location) : StatementNode(Location), IReaxDeclaration
{
    public override IReaxNode[] Children => [Declaration, Array, Context];

    public void Initialize(IReaxExecutionContext context)
    {
        Declaration.Initialize(context);
    }
    
    public override void Execute(IReaxExecutionContext context)
    {
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
        else if (Array is StructFieldAccessNode structFieldAccess)
            return (ArrayNode)structFieldAccess.Evaluation(context);
        else
            return (ArrayNode)Array;

    }

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);

        Declaration.Serialize(writer);
        Array.Serialize(writer);
        Context.Serialize(writer);
        base.Serialize(writer);
    }

    public static new ForInNode Deserialize(BinaryReader reader)
    {
        var declaration = BinaryDeserializerHelper.Deserialize<DeclarationNode>(reader);
        var array = BinaryDeserializerHelper.Deserialize<ReaxNode>(reader);
        var context = BinaryDeserializerHelper.Deserialize<ContextNode>(reader);
        var location = ReaxNode.Deserialize(reader);
        return new ForInNode(declaration, array, context, location);
    }

    public override string ToString()
    {
        return $"for {Declaration.Identifier}:{Declaration.Type} in {Array} {{}}";
    }

}
