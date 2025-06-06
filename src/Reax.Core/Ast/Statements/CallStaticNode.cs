using System;
using Reax.Core.Ast.Literals;
using Reax.Core.Helpers;
using Reax.Core.Locations;

namespace Reax.Core.Ast.Statements;

public record CallStaticNode(
    string TypeName,
    string MethodName,
    ReaxNode[] Arguments,
    SourceLocation Location)
    : StatementNode(Location)
{
    public override IReaxNode[] Children => Arguments;

    public override void Execute(IReaxExecutionContext context)
    {
        try
        {
            TryExecute(context);
        }
        catch (Exception ex)
        {
            throw new ReturnErrorException(new StringNode(ex.InnerException?.Message ?? ex.Message, Location));
        }
    }

    private void TryExecute(IReaxExecutionContext context)
    { 
        var type = TypeResolverHelper.GetTypeFromName(TypeName);
        if (type == null)
        {
            throw new InvalidOperationException($"Type '{TypeName}' not found.");
        }

        var parameters = Arguments.Select(x => x.GetValue(context).Value).ToArray();
        var parameterTypes = parameters.Select(x => x.GetType()).ToArray();
        var nativeMember = TypeResolverHelper.GetMemberInfo(type, MethodName, parameterTypes);
        if (nativeMember is null)
            return;

        TypeResolverHelper.InvokeMember(null, nativeMember, parameters);
    }

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");
        writer.Write(typename);

        writer.Write(TypeName);
        writer.Write(MethodName);

        writer.Write(Arguments.Length);
        foreach (var argument in Arguments)
            argument.Serialize(writer);

        base.Serialize(writer);
    }
    
    public static new CallStaticNode Deserialize(BinaryReader reader)
    {
        var typeName = reader.ReadString();
        var methodName = reader.ReadString();
        var argumentLength = reader.ReadInt32();
        var arguments = new ReaxNode[argumentLength];
        for (int i = 0; i < argumentLength; i++)
            arguments[i] = BinaryDeserializerHelper.Deserialize<ReaxNode>(reader);

        var location = ReaxNode.Deserialize(reader);
        return new CallStaticNode(typeName, methodName, arguments, location);
    }
    
}
