using System;
using System.Collections.Concurrent;
using Reax.Core.Locations;

namespace Reax.Core.Debugger;

public class DebuggerArgs
{
    public DebuggerArgs(
        IEnumerable<DebuggerModel> models, 
        string stackTrace, 
        SourceLocation location)
    {
        Models = models;
        StackTrace = stackTrace;
        Location = location;
    }

    public IEnumerable<DebuggerModel> Models { get; private set; }
    public string StackTrace { get; private set; }
    public SourceLocation Location { get; private set; }
}
