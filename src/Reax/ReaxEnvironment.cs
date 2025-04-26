using System;
using Reax.Interpreter;
using Reax.Parser.Node;
using Reax.Runtime.Registries;

namespace Reax;

public static class ReaxEnvironment
{
    public static bool Debug { get; set; } = false;
    public static ReaxInterpreter? MainInterpreter { get; set; }
    public static string DirectoryRoot { get; set; } = string.Empty;
    public static Dictionary<string, ScriptNode> ImportedFiles { get; set; } = new();
    public static Dictionary<string, HashSet<int>> BreakPoints = new();
    public static BuiltInRegistry BuiltInRegistry = new();
}
