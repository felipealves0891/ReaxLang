using System;
using Reax.Parser;
using Reax.Parser.Node;
using Reax.Parser.Node.Literals;
using Reax.Semantic;
using Reax.Semantic.Symbols;

namespace Reax.Tests.Semantic.ReaxResults;

public class SemanticAssignmentNodeTests : ReaxResultTests
{
    [Fact]
    public void Validate_HappyWay_ReaxValue()
    {
        //Arrange
        var identifier = Guid.NewGuid().ToString();
        var variable = new VarNode(identifier, Parser.DataType.NONE, new SourceLocation());
        var value = new NumberNode("0", new SourceLocation());
        var assignment = new AssignmentNode(variable, value, new SourceLocation());

        var symbol = new Symbol(identifier, DataType.NUMBER, SymbolCategory.LET, new SourceLocation(), null, false, true);
        MockedContext.Setup(x => x.GetSymbol(identifier))
                     .Returns(symbol);

        //Act
        var result = assignment.Validate(MockedContext.Object);

        //Assert
        Assert.Empty(result.Message);
        Assert.True(result.Status);
    }

    [Fact]
    public void Validate_InvalidType_ReaxValue()
    {
        //Arrange
        var identifier = Guid.NewGuid().ToString();
        var variable = new VarNode(identifier, Parser.DataType.NONE, new SourceLocation());
        var value = new StringNode("0", new SourceLocation());
        var assignment = new AssignmentNode(variable, value, new SourceLocation());

        var symbol = new Symbol(identifier, DataType.NUMBER, SymbolCategory.LET, new SourceLocation(), null, false, true);
        MockedContext.Setup(x => x.GetSymbol(identifier))
                     .Returns(symbol);

        //Act
        var result = assignment.Validate(MockedContext.Object);

        //Assert
        Assert.Contains("Atribuição de tipo incompativel no identificador ", result.Message);
        Assert.False(result.Status);
    }
    
    [Fact]
    public void Validate_HappyWay_ReaxResult()
    {
        //Arrange
        var identifier = Guid.NewGuid().ToString();
        var variable = new VarNode(identifier, DataType.NONE, new SourceLocation());
    
        var symbol = new Symbol(identifier, DataType.STRING, SymbolCategory.LET, new SourceLocation(), null, false, true);
        MockedContext.Setup(x => x.GetSymbol(identifier))
                     .Returns(symbol);

        var callChildren = false;
        var value = new MockReaxNodeResult((context, type) => 
        {
            callChildren = true;
            return ValidationResult.Success(new SourceLocation());
        });

        var assignment = new AssignmentNode(variable, value, new SourceLocation());

        //Act
        var result = assignment.Validate(MockedContext.Object, DataType.STRING);

        //Assert
        Assert.True(callChildren);
        Assert.Empty(result.Message);
        Assert.True(result.Status);
    }

    [Fact]
    public void Validate_Undeclared()
    {
        //Arrange
        var identifier = Guid.NewGuid().ToString();
        var variable = new VarNode(identifier, DataType.NONE, new SourceLocation());
        var value = new StringNode("0", new SourceLocation());
        var assignment = new AssignmentNode(variable, value, new SourceLocation());

        //Act
        var result = assignment.Validate(MockedContext.Object, DataType.STRING);

        //Assert
        Assert.EndsWith("não foi declarado, mas esta sendo usado!", result.Message);
        Assert.False(result.Status);
    }
}
