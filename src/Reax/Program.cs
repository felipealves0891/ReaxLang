using Reax;
using Reax.Interpreter;
using Reax.Lexer;
using Reax.Lexer.Readers;
using Reax.Parser;

var fileInfo = new FileInfo(@"D:\Source\scripts\simple.reax");
ReaxEnvironment.DirectoryRoot = fileInfo.DirectoryName ?? throw new Exception();


var code = File.ReadAllText(fileInfo.FullName);
var lexer = new ReaxLexer(new ReaxTextReader(code));
var tokens = lexer.Tokenize();

var parser = new ReaxParser(tokens);
var ast = parser.Parse();

var interpreter = new ReaxInterpreterBuilder()
                        .AddFunctionsBuiltIn()
                        .BuildMain(ast.ToArray());
                        
interpreter.Interpret();
