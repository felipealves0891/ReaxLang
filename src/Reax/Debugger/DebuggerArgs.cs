using System;
using System.Collections.Concurrent;
using Reax.Core.Locations;
using Reax.Parser;
using Reax.Parser.Node;

namespace Reax.Debugger;

public class DebuggerArgs
{
    public DebuggerArgs(
        IEnumerable<DebuggerModel> models, 
        IEnumerable<ReaxNode> stackTrace, 
        SourceLocation location)
    {
        Models = models;
        StackTrace = stackTrace;
        Location = location;
    }

    public IEnumerable<DebuggerModel> Models { get; private set; }
    public IEnumerable<ReaxNode> StackTrace { get; private set; }
    public SourceLocation Location { get; private set; }
}
