using System;
using Moq;
using Reax.Lexer;
using Reax.Parser.NodeParser;

namespace Reax.Tests;

public abstract class BaseTest<Tested>
{
    protected abstract Tested CreateTested();

    public ITokenSource CreateTokenSource(Token[] tokens, Action<IMock<ITokenSource>, int>? configuration = null) 
    {
        var mockedTokenSource = new Mock<ITokenSource>();
        var position = 0;
        var defaultToken = new Token(TokenType.UNKNOW, (byte)' ', "", 0, 0);

        mockedTokenSource.Setup(x => x.Advance())
                         .Callback(() => {
                            position++;
                         });

        mockedTokenSource.Setup(x => x.BeforeToken)
                         .Returns(() => position < tokens.Length ? tokens[position] : defaultToken);

        mockedTokenSource.Setup(x => x.CurrentToken)
                         .Returns(() => tokens[position]);
                         
        mockedTokenSource.Setup(x => x.NextToken)
                         .Returns(() => position+1 > tokens.Length ? tokens[position] : defaultToken);

        mockedTokenSource.Setup(x => x.EndOfTokens)
                         .Returns(() => position >= tokens.Length);

        configuration?.Invoke(mockedTokenSource, position);
        return mockedTokenSource.Object;
    }
}
