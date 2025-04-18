using System;
using Reax.Interpreter;
using Reax.Parser.Node;

namespace Reax;

public static class ReaxEnvironment
{
    public static ReaxInterpreter? MainInterpreter { get; set; }
    public static string DirectoryRoot { get; set; } = string.Empty;
    public static IDictionary<string, ScriptNode> ImportedFiles { get; set; } = new Dictionary<string, ScriptNode>();
}
