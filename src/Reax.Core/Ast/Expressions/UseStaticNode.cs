using System;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Ast.Objects;
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

    public override string ToString()
    {
        return $" {Member} of {TypeName}";
    }
}
