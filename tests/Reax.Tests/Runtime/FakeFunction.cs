using System;
using Reax.Parser.Node;
using Reax.Runtime.Functions;

namespace Reax.Tests.Runtime;

public class FakeFunction : Function
{
    private readonly Func<ReaxNode[], ReaxNode>? action;

    public FakeFunction(Func<ReaxNode[], ReaxNode>? action = null)
    {
        this.action = action;
    }

    public override (ReaxNode? Success, ReaxNode? Error) Invoke(params ReaxNode[] parameters)
    {
        if(action is not null)
            return (action(parameters), null);

        return (new NullNode(new Parser.SourceLocation()), null);
    }
}
