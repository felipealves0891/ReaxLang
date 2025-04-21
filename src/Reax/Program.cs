using Reax;
using Reax.Debugger;
using Reax.Interpreter;
using Reax.Semantic.Scopes;

var fileInfo = new FileInfo(@"D:\Source\scripts\simple.reax");
ReaxEnvironment.DirectoryRoot = fileInfo.DirectoryName ?? throw new Exception();
var interpreter = ReaxCompiler.Compile(fileInfo.FullName);

try
{
    interpreter.Interpret();
    var columns = ReaxScope.Table.PrintTableHeader();
    ReaxScope.Table.PrintTable(columns);
}
catch (System.Exception ex)
{
    Console.WriteLine(ex); 
    Console.WriteLine("Reax Error:");
    interpreter.PrintStackTrace();
}

