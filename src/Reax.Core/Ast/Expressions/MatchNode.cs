using System;
using System.Diagnostics.CodeAnalysis;
using Reax.Core.Locations;
using Reax.Core.Ast.Statements;
using Reax.Core.Ast.Literals;
using Reax.Core.Debugger;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Helpers;

namespace Reax.Core.Ast.Expressions;

[ExcludeFromCodeCoverage]
public record MatchNode(
    ExpressionNode Expression,    
    ActionNode Success,
    ActionNode Error,  
    SourceLocation Location) : ExpressionNode(Location)
{
    public override IReaxNode[] Children => [Expression, Success, Error];

    public override IReaxValue Evaluation(IReaxExecutionContext context)
    {
        var result = Expression.Evaluation(context);
        if (result.Type == Success.Parameter.Type)
        {
            var successInterpreter = context.CreateInterpreter(Success.ToString(), [Success.Context], [Success.Parameter]);
            successInterpreter.Interpret("Success", false, result);
            return successInterpreter.Output ?? throw new InvalidOperationException($"{Location} - Era esperado um retorno de sucesso ou erro, mas não teve nenhum retorno!");
        }
        else if (result.Type == Error.Parameter.Type)
        {
            var errorInterpreter = context.CreateInterpreter(Error.ToString(), [Error.Context], [Error.Parameter]);
            errorInterpreter.Interpret("Error", false, result);
            return errorInterpreter.Output ?? throw new InvalidOperationException($"{Location} - Era esperado um retorno de sucesso ou erro, mas não teve nenhum retorno!");
        }
        else
            throw new InvalidOperationException($"{Location} - Era esperado um retorno de sucesso ou erro, mas não teve nenhum retorno!");
    }
    
    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);

        Expression.Serialize(writer);
        Success.Serialize(writer);
        Error.Serialize(writer);
        base.Serialize(writer);
    }

    public static new MatchNode Deserialize(BinaryReader reader)
    {
        var expression = BinaryDeserializerHelper.Deserialize<ExpressionNode>(reader);
        var success = BinaryDeserializerHelper.Deserialize<ActionNode>(reader);
        var error = BinaryDeserializerHelper.Deserialize<ActionNode>(reader);
        var location = ReaxNode.Deserialize(reader);
        return new MatchNode(expression, success, error, location);
    }

    public override string ToString()
    {
        return $"match {Expression} {{ success, error }}";
    }
}
