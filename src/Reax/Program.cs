using Reax.Interpreter;
using Reax.Lexer;
using Reax.Parser;

var code = @"
let minha_variavel = 'Meu Texto';
let meuNumero = 5;
if meuNumero < 0 {
    writer('Meu numero é maior que 0');    
} else {
    writer('Meu numero é menor que 10');    
}
minha_variavel = 'Meu texto alterado ';
writer(minha_variavel);
writer(meuNumero);
";

var lexer = new ReaxLexer(code);
var tokens = lexer.Tokenize();

var parser = new ReaxParser(tokens);
var ast = parser.Parse().ToArray();

var interpreter = new ReaxInterpreter(ast);
interpreter.Interpret();

Console.Read();
