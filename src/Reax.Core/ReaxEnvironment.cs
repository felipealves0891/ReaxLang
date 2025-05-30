using Reax.Core;
using Reax.Core.Ast.Statements;

namespace Reax.Core;

public static class ReaxEnvironment
{
    public static bool Debug { get; set; } = false;
    public static IReaxInterpreter? MainInterpreter { get; set; }
    public static string DirectoryRoot { get; set; } = Environment.CurrentDirectory;
    public static Dictionary<string, ScriptNode> ImportedFiles { get; set; } = new();
}
