using System;
using Reax.Interpreter;

namespace Reax;

public static class ReaxEnvironment
{
    public static ReaxInterpreter? MainInterpreter { get; set; }
    public static string DirectoryRoot { get; set; } = string.Empty;
    public static ISet<string> ImportedFiles { get; set; } = new HashSet<string>();
}
