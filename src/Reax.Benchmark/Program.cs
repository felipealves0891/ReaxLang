using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;
using Reax.Interpreter;
using Reax.Lexer;
using Reax.Parser;

namespace Reax.Benchmark;

[MemoryDiagnoser]
public class Program
{
    static void Main(string[] args)
    {
        BenchmarkRunner.Run<Program>();
    }

    [Benchmark]
    public void ReaxRun()
    {
        var fileInfo = new FileInfo(@"D:\Source\scripts\simple.reax");

        var code = File.ReadAllText(fileInfo.FullName);

        var lexer = new ReaxLexer(code);
        var tokens = lexer.Tokenize();

        var parser = new ReaxParser(tokens);
        var ast = parser.Parse();

        var interpreter = new ReaxInterpreterBuilder()
                                .AddFunctionsBuiltIn()
                                .BuildMain(ast.ToArray());
                                
        interpreter.Interpret();
    }
}

/*

1º
| Method  | Mean     | Error    | StdDev   | Median   | Gen0      | Gen1     | Allocated |
|-------- |---------:|---------:|---------:|---------:|----------:|---------:|----------:|
| ReaxRun | 48.01 ms | 5.091 ms | 14.36 ms | 42.66 ms | 1000.0000 | 142.8571 |   3.09 MB |

*/