using System;
using System.Collections.Concurrent;
using Reax.Parser;
using Reax.Parser.Node;

namespace Reax.Debugger;

public class DebuggerArgs
{
    public DebuggerArgs(
        IEnumerable<DebuggerModel> models, 
        ConcurrentStack<ReaxNode> stackTrace, 
        SourceLocation location)
    {
        Models = models;
        StackTrace = stackTrace;
        Location = location;
    }

    public IEnumerable<DebuggerModel> Models { get; private set; }
    public ConcurrentStack<ReaxNode> StackTrace { get; private set; }
    public SourceLocation Location { get; private set; }
}
