using System;
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
}
