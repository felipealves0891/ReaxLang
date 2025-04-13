using Reax.Interpreter;
using Reax.Lexer;
using Reax.Parser;

var code = File.ReadAllText(@"D:\Source\simple.reax");

var lexer = new ReaxLexer(code);
var tokens = lexer.Tokenize();

var parser = new ReaxParser(tokens);
var ast = parser.Parse().ToArray();

var interpreter = new ReaxInterpreter(ast);
interpreter.Interpret();
