using System;
using Moq;
using Reax.Parser;
using Reax.Parser.Node;
using Reax.Semantic;
using Reax.Semantic.Symbols;

namespace Reax.Tests.Semantic.ReaxResults;

public class SemanticActionNodeTests : ReaxResultTests
{
    [Fact]
    public void Validate_HapyWay() 
    {
        //Arrange
        var location = new SourceLocation();
        var context = new MockReaxNodeResult((context, type) => ValidationResult.Success(location));
        var parameter = new VarNode(Guid.NewGuid().ToString(), DataType.STRING, location);
        var action = new ActionNode([parameter], context, DataType.STRING, location);

        MockedContext.Setup(x => x.SetSymbol(
            It.Is<Symbol>(symbol => symbol.Identifier == parameter.Identifier 
                       && symbol.Type == parameter.Type
                       && symbol.Category == SymbolCategory.PARAMETER)))
                    .Returns(ValidationResult.Success(location))
                    .Verifiable();

        //Act
        var result = action.Validate(MockedContext.Object, DataType.STRING);

        //Assert
        Assert.Empty(result.Message);
        Assert.True(result.Status);
        Assert.Equal(2, result.TotalResults);
        MockedContext.Verify();

    }
}
