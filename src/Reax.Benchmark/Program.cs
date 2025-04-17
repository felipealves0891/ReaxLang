using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;
using Reax.Interpreter;
using Reax.Lexer;
using Reax.Lexer.Readers;
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
    public void TextLexer()
    {
        var fileInfo = new FileInfo(@"D:\Source\scripts\simple.reax");
        ReaxEnvironment.DirectoryRoot = fileInfo.DirectoryName ?? throw new Exception();

        var code = File.ReadAllText(fileInfo.FullName);
        var reader = new ReaxTextReader(code);

        var lexer = new ReaxLexer(reader);
        lexer.Tokenize().ToArray();
    }
    
    [Benchmark]
    public void StreamLexer()
    {
        var fileInfo = new FileInfo(@"D:\Source\scripts\simple.reax");
        ReaxEnvironment.DirectoryRoot = fileInfo.DirectoryName ?? throw new Exception();

        var lexer = new ReaxLexer(new ReaxStreamReader(fileInfo.FullName));
        lexer.Tokenize().ToArray();
    }
}

/*

1º
| Method  | Mean     | Error    | StdDev   | Median   | Gen0      | Gen1     | Allocated |
|-------- |---------:|---------:|---------:|---------:|----------:|---------:|----------:|
| ReaxRun | 48.01 ms | 5.091 ms | 14.36 ms | 42.66 ms | 1000.0000 | 142.8571 |   3.09 MB |

2º
| Method    | Mean     | Error    | StdDev   | Median   | Gen0      | Gen1     | Allocated |
|---------- |---------:|---------:|---------:|---------:|----------:|---------:|----------:|
| TextRun   | 49.30 ms | 2.163 ms | 6.172 ms | 48.49 ms | 1200.0000 |        - |   4.22 MB |
| StreamRun | 40.52 ms | 2.523 ms | 7.438 ms | 37.31 ms | 1333.3333 | 111.1111 |   4.22 MB |

*/