using Reax;
using Reax.Debugger;
using Reax.Interpreter;

var fileInfo = new FileInfo(@"D:\Source\scripts\simple.reax");
ReaxEnvironment.DirectoryRoot = fileInfo.DirectoryName ?? throw new Exception();
ReaxEnvironment.Debug = false;
ReaxEnvironment.BreakPoints[@"D:\Source\scripts\simple.reax"] = new HashSet<int>([60]);
var interpreter = ReaxCompiler.Compile(fileInfo.FullName);

try
{
    
    interpreter.Interpret();
}
catch (System.Exception ex)
{
    Logger.LogError(ex, "Error: "); 
    Console.WriteLine("Reax Error:");
    interpreter.PrintStackTrace();
}