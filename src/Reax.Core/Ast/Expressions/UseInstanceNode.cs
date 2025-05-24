using System;
using System.Collections.Immutable;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Literals;
using Reax.Core.Ast.Objects;
using Reax.Core.Locations;
using Reax.Core.Types;
using Reax.Parser.Node;

namespace Reax.Core.Ast.Expressions;

public record UseInstanceNode(
    string Member,
    ReaxNode[] Arguments,
    ReaxNode Target,
    SourceLocation Location) : ExpressionNode(Location)
{
    public override IReaxNode[] Children => [.. Arguments, Target];

    public override IReaxValue Evaluation(IReaxExecutionContext context)
    {
        var nativeTarget = GetNativeTarget(context, Target);
        var nativeTargetType = nativeTarget.GetType();

        var parameters = Arguments.Select(x => x.GetValue(context).Value).ToArray();
        var parameterTypes = parameters.Select(x => x.GetType()).ToArray();
        var nativeMember = TypeResolverHelper.GetMemberInfo(nativeTargetType, Member, parameterTypes);
        if (nativeMember is null)
            return new NullNode(Location);

        var result = TypeResolverHelper.InvokeMember(nativeTarget, nativeMember, parameters);
        if (result is null)
            return new NullNode(Location);

        return new NativeValueNode(result);
    }

    private static object GetNativeTarget(IReaxExecutionContext context, ReaxNode Target)
    {
        if (Target is LiteralNode literal)
            return literal.Value;
        else if (Target is ExpressionNode expression)
            return expression.Evaluation(context).Value;
        else
            throw new InvalidOperationException("Target é invalido para chamada nativa");
    }

    public override void Serialize(BinaryWriter writer)
    {
        var typename = GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException("Tipo nulo ao serializar");

        writer.Write(typename);

        writer.Write(Member);
        writer.Write(Arguments.Length);
        foreach (var arg in Arguments)
        {
            arg.Serialize(writer);
        }
        Target.Serialize(writer);
        base.Serialize(writer);
    }

    public override string ToString()
    {
        var parameters = string.Join(',', Arguments.Select(x => x.ToString()));
        return $"use {Member}({parameters}) in {Target};";
    }
}
