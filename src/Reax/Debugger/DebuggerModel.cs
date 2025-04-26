using System;

namespace Reax.Debugger;

public class DebuggerModel
{
    public string Name { get; set; } = string.Empty;

    public string Identifier { get; set; }= string.Empty;

    public string Immutable { get; set; } = string.Empty;

    public string Bind { get; set; } = string.Empty;

    public string Async { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Context { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

}
