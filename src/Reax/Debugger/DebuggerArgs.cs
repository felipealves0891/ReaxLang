using System;
using Reax.Parser;
using Reax.Parser.Node;

namespace Reax.Debugger;

public class DebuggerArgs
{
    public DebuggerArgs(
        IEnumerable<DebuggerModel> models, 
        Stack<ReaxNode> stackTrace, 
        SourceLocation location)
    {
        Models = models;
        StackTrace = stackTrace;
        Location = location;
    }

    public IEnumerable<DebuggerModel> Models { get; private set; }
    public Stack<ReaxNode> StackTrace { get; private set; }
    public SourceLocation Location { get; private set; }
}
