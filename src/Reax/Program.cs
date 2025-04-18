using Reax;
using Reax.Interpreter;

var fileInfo = new FileInfo(@"D:\Source\scripts\simple.reax");
ReaxEnvironment.DirectoryRoot = fileInfo.DirectoryName ?? throw new Exception();
var interpreter = ReaxCompiler.Compile(fileInfo.FullName);

try
{
    interpreter.Interpret();    
}
catch (System.Exception ex)
{
    Console.WriteLine(ex.Message);
    interpreter.PrintStackTrace();
}
