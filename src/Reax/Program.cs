using Reax.Interpreter;
using Reax.Lexer;
using Reax.Parser;

var code = @"
let minha_variavel = 'Meu Texto';
on minha_variavel {
    writer('Minha variavel foi alterada!');
}
let meuNumero = 5;
if meuNumero != 10 {
    writer('Meu numero é igual a 5');    
} else {
    writer('Meu numero é menor que 10');    
}
writer(minha_variavel);
minha_variavel = 'Meu texto alterado ';
writer(meuNumero);
";

var lexer = new ReaxLexer(code);
var tokens = lexer.Tokenize();

var parser = new ReaxParser(tokens);
var ast = parser.Parse().ToArray();

var interpreter = new ReaxInterpreter(ast);
interpreter.Interpret();
