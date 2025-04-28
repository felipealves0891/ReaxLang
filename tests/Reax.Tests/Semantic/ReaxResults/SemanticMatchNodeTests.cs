using System;
using Moq;
using Reax.Parser;
using Reax.Parser.Node;
using Reax.Parser.Node.Literals;
using Reax.Parser.Node.Operations;
using Reax.Semantic;
using Reax.Semantic.Symbols;

namespace Reax.Tests.Semantic.ReaxResults;

public class SemanticMatchNodeTests : ReaxResultTests
{
    [Fact]
    public void Validate_HappyWay() 
    {
        //Arrange
        var location = new SourceLocation();
        var expression = new MockReaxNodeResult((context, type) => ValidationResult.Success(location));
        var success = new ActionNode([new VarNode("", DataType.NONE, location)], expression, DataType.STRING, location);
        var error = new ActionNode([new VarNode("", DataType.NONE, location)], expression, DataType.STRING, location);
        var match = new MatchNode(expression, success, error, location);

        MockedContext.Setup(x => x.SetSymbol(It.IsAny<Symbol>()))
                     .Returns(ValidationResult.Success(location));

        //Act
        var result = match.Validate(MockedContext.Object, DataType.STRING);

        //Assert
        Assert.Empty(result.Message);
        Assert.True(result.Status);
        Assert.Equal(3, result.TotalResults);
    }

}
