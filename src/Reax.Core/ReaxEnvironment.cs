using Reax.Core;
using Reax.Core.Ast.Statements;

namespace Reax.Core;

public static class ReaxEnvironment
{
    public static string DirectoryRoot { get; set; } = string.Empty;
    public static Dictionary<string, ScriptNode> ImportedFiles { get; set; } = new();
}
