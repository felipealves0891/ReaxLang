using System;
using Moq;
using Reax.Parser;
using Reax.Parser.Node;
using Reax.Parser.Node.Interfaces;
using Reax.Semantic;
using Reax.Semantic.Symbols;

namespace Reax.Tests.Semantic.ReaxResults;

public class SemanticDeclarationNodeTests : ReaxResultTests
{
    [Theory]
    [InlineData(false, SymbolCategory.LET)]
    [InlineData(true, SymbolCategory.CONST)]
    public void Validate_HappyWay_DeclarationOnly(bool immutable, SymbolCategory category) 
    {
        //Arrange
        var identifier = Guid.NewGuid().ToString();
        var async = true;
        var type = DataType.STRING;
        var declaration = new DeclarationNode(identifier, immutable, async, type, null, new SourceLocation());

        MockedContext.Setup(x => x.SetSymbol(It.Is<Symbol>(symbol => symbol.Category == category && symbol.Immutable == immutable)))
                      .Returns(ValidationResult.Success(declaration.Location))
                      .Verifiable();

        //Act
        var result = declaration.Validate(MockedContext.Object);

        //Assert
        Assert.True(result.Status);
        Assert.Empty(result.Message);
        Assert.Equal(declaration.Location, result.Source);

        MockedContext.Verify();
    }

    [Theory]
    [InlineData(false, SymbolCategory.LET)]
    [InlineData(true, SymbolCategory.CONST)]
    public void Validate_SemanticError_AlreadyDeclaration(bool immutable, SymbolCategory category) 
    {
        //Arrange
        var identifier = Guid.NewGuid().ToString();
        var async = true;
        var type = DataType.STRING;
        var declaration = new DeclarationNode(identifier, immutable, async, type, null, new SourceLocation());

        MockedContext.Setup(x => x.SetSymbol(It.Is<Symbol>(symbol => symbol.Category == category && symbol.Immutable == immutable)))
                      .Returns(ValidationResult.ErrorAlreadyDeclared(identifier, declaration.Location))
                      .Verifiable();

        //Act
        var result = declaration.Validate(MockedContext.Object);

        //Assert
        Assert.False(result.Status);
        Assert.Contains("já foi declarado!", result.Message);
        Assert.Equal(declaration.Location, result.Source);
        MockedContext.Verify();
    }

    [Theory]
    [InlineData(false, SymbolCategory.LET)]
    [InlineData(true, SymbolCategory.CONST)]
    public void Validate_HappyWay_DeclarationAndAssignment(bool immutable, SymbolCategory category) 
    {
        //Arrange
        var identifier = Guid.NewGuid().ToString();
        var async = true;
        var type = DataType.STRING;
        var callAssignmentValidate = false;
        var assignment = new MockReaxNodeResult((context, currentType) => 
        {
            callAssignmentValidate = true;
            Assert.Equal(type, currentType);
            return ValidationResult.Success(new SourceLocation());
        });

        MockedContext.Setup(x => x.EnterFrom(identifier))
                      .Returns(MockedDisposable.Object)
                      .Verifiable();

        var declaration = new DeclarationNode(identifier, immutable, async, type, assignment, new SourceLocation());
        MockedContext.Setup(x => x.SetSymbol(It.Is<Symbol>(symbol => symbol.Category == category && symbol.Immutable == immutable)))
                      .Returns(ValidationResult.Success(declaration.Location))
                      .Verifiable();

        //Act
        var result = declaration.Validate(MockedContext.Object);

        //Assert
        Assert.True(callAssignmentValidate);
        Assert.True(result.Status);
        Assert.Empty(result.Message);
        Assert.Equal(declaration.Location, result.Source);
        MockedContext.Verify();
        MockedDisposable.Verify(x => x.Dispose());
    }
}
