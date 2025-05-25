using System;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Objects;
using Reax.Core.Helpers;
using Reax.Core.Locations;
using Reax.Core.Types;
using Reax.Parser.Node;

namespace Reax.Core.Ast.Expressions;

public record UseStaticNode(
    string Member,
    string TypeName,
    ReaxNode[] Arguments,
    SourceLocation Location) : ExpressionNode(Location)
{
    public override IReaxNode[] Children => Arguments;

    public override IReaxValue Evaluation(IReaxExecutionContext context)
    {
        var nativeType = TypeResolverHelper.GetTypeFromName(TypeName);
        if (nativeType is null)
            return new NullNode(Location);

        var nativeMember = TypeResolverHelper.GetMemberInfo(nativeType, Member);
        if (nativeMember is null)
            return new NullNode(Location);

        var parameters = Arguments.Select(x => x.GetValue(context).Value).ToArray();
        var result = TypeResolverHelper.InvokeMember(null, nativeMember, parameters);
        if (result is null)
            return new NullNode(Location);

        return new NativeValueNode(result);
    }

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);

        writer.Write(Member);
        writer.Write(TypeName);
        writer.Write(Arguments.Length);
        foreach (var arg in Arguments)
        {
            arg.Serialize(writer);
        }
        base.Serialize(writer);
    }
    
    public static new UseStaticNode Deserialize(BinaryReader reader)
    {
        var member = reader.ReadString();
        var typeName = reader.ReadString();
        var argumentCount = reader.ReadInt32();
        var arguments = new ReaxNode[argumentCount];
        for (int i = 0; i < argumentCount; i++)
        {
            arguments[i] = BinaryDeserializerHelper.Deserialize<ReaxNode>(reader);
        }
        var location = ReaxNode.Deserialize(reader);
        return new UseStaticNode(member, typeName, arguments, location);
    }

    public override string ToString()
    {
        return $" {Member} of {TypeName}";
    }
}
