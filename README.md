
# Bem-vindo ao Reax

Reax é uma linguagem de programação reativa e estática com sintaxe simples, foco em legibilidade e execução orientada a eventos. Ela permite que você escreva código expressivo com reações automáticas a mudanças de estado.

  

### Por que usar a Reax?

  

- Sintaxe intuitiva e expressiva

- Paradigma reativo nativo

- Tipagem estática com inferência

- Executa via interpretador em C#

  

## Referencias
 

### Variáveis podem ser observadas e sempre que alterada gera um evento

```
let contador: int = 0;
```

### Constantes são variáveis imutáveis, podendo manter a segurança sem o medo de imprevistos
```
const limite: int = 10;
```

### Vínculos são variáveis calculadas automaticamente sempre que são recuperada
```
 bind dobro: float -> contador / 2;
```

>Não é possivel utilizar um bind em outro que dependa dele exemplo:
>**bind contador -> contador + 1;**
>Isso ira gerar um erro em tempo de analise, devido a referencias circulares.

>Vinculos não podem ser observados, pois, não são reatribuidos.
  

### Observadores de alterações de variáveis
```
on contador -> console.writer('Contando: {0}', contador);
```
>Não é possivel alterar o valor da variavel observavel em um evento dispara
>**on contador {
>&nbsp;&nbsp;&nbsp;&nbsp;contador = contador + 1;
>};**
>Isso ira gerar um erro em tempo de analise, devido a referencias circulares.
  
### Filtros de execuções de observadores

```
on contador when contador > 10 {
	console.writer('O contador passou dos 10!');
}
```

### Condicional If
```
if meuNumero > 5 {
	console.writer('Meu numero é igual a 5');
} else {
	console.writer('Meu numero é menor que 10');
}
```

### Estrutura de repetições For
```
for controle: int = 0 to 100 {
	meuNumero = calculate.sum(meuNumero,1);
}
```

### Estrutura de repetições While
```
while controle < 100 and controle > 0) {
	meuNumero = calculate.sum(meuNumero,1);
	controle = controle + 1;
}
```

### Observadores assíncronos, os observadores são chamados de forma assíncrona
```
async let minha_variavel = 'Meu Texto';
```

### Importar arquivos de scripts Reax
```
 import script 'calculate.reax';
```

### Importar módulos embutidos no Reax
```
import module 'console';
```

### Funções
As funções podem retornar dois tipos diferentem, um em caso e sucesso e outro em caso de erro.
Caso não seja declarado o tipo de erro, é inferido como void

```
fun divider(num: int, den: int): int | string {
	if(den == 0) {
		return error 'Tentativa de divisão por 0';
	}
	return success num / den + meuValorInterno;
}
```

### Match
Para invoca uma função com tipo de erro diferente de void, deve se usar o comando ```match```
```
let minhaDivisao: int = match calculate.divider(50, 0) {
	success (val: int): int -> val;
	error (message: string): int {
		console.writer(message);
		return -1;
	}
}
```

### Comentário
> Para usar comentários você só precisa de um # antes do texto comentado, o resto da lina será entendido como um comentario

```
# Meu comentario de uma linha
```

```
const meuValorImutavel: string = 'Sempre assim'; # uma constante e um comentário
```
