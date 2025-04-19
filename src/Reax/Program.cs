using Reax;
using Reax.Debugger;
using Reax.Interpreter;

var fileInfo = new FileInfo(@"D:\Source\scripts\simple.reax");
ReaxEnvironment.DirectoryRoot = fileInfo.DirectoryName ?? throw new Exception();
var interpreter = ReaxCompiler.Compile(fileInfo.FullName);

try
{
    interpreter.Interpret();    
    var columns = ReaxEnvironment.Symbols.Registry.PrintTableHeader();
    ReaxEnvironment.Symbols.Registry.PrintTable(columns);
}
catch (System.Exception ex)
{
    Console.WriteLine(ex); 
    Console.WriteLine("Reax Error:");
    interpreter.PrintStackTrace();
}

