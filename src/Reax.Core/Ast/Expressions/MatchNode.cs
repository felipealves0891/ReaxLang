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
        try
        {
            var result = Expression.Evaluation(context);
            var nodes = Success.Context is ContextNode contextNode ? contextNode.Block : [Success.Context];
            var successInterpreter = context.CreateInterpreter(Success.ToString(), nodes, [Success.Parameter]);
            successInterpreter.Interpret("Success", false, result);
            return successInterpreter.Output ?? throw new InvalidOperationException($"{Location} - Era esperado um retorno de sucesso ou erro, mas não teve nenhum retorno!");
        }
        catch (ReturnErrorException ex)
        {
            var nodes = Error.Context is ContextNode contextNode ? contextNode.Block : [Error.Context];
            var errorInterpreter = context.CreateInterpreter(Error.ToString(), nodes, [Error.Parameter]);
            errorInterpreter.Interpret("Error", false, ex.Value);
            return errorInterpreter.Output ?? throw new InvalidOperationException($"{Location} - Era esperado um retorno de sucesso ou erro, mas não teve nenhum retorno!");
        }
        catch (Exception ex)
        { 
            var nodes = Error.Context is ContextNode contextNode ? contextNode.Block : [Error.Context];
            var errorInterpreter = context.CreateInterpreter(Error.ToString(), nodes, [Error.Parameter]);
            errorInterpreter.Interpret("Error", false, new StringNode(ex.InnerException?.Message ?? ex.Message, Location));
            return errorInterpreter.Output ?? throw new InvalidOperationException($"{Location} - Era esperado um retorno de sucesso ou erro, mas não teve nenhum retorno!");
        }
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
