using Reax.Interpreter;
using Reax.Lexer;
using Reax.Lexer.Readers;
using Reax.Parser;

/*
Proximos:
- Adicionar Stream de caracteres
- Alterar Parser para ler do stream
- Adicionar os importes como pila para que os mais novos seja processados primeiro
*/

var fileInfo = new FileInfo(@"D:\Source\scripts\simple.reax");
var code = File.ReadAllText(fileInfo.FullName);

var lexer = new ReaxLexer(new ReaxTextReader(code));
var tokens = lexer.Tokenize();

var parser = new ReaxParser(tokens);
var ast = parser.Parse();

var interpreter = new ReaxInterpreterBuilder()
                        .AddFunctionsBuiltIn()
                        .Build(ast.ToArray());
                        
interpreter.Interpret();
