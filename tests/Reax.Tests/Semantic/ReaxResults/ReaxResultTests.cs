using System;
using Moq;
using Reax.Parser.Node.Interfaces;

namespace Reax.Tests.Semantic.ReaxResults;

public abstract class ReaxResultTests
{    
    public ReaxResultTests()
    {
        MockedContext = new Mock<ISemanticContext>();
        MockedDisposable = new Mock<IDisposable>();
    }

    protected Mock<ISemanticContext> MockedContext { get; init; }
    protected Mock<IDisposable> MockedDisposable { get; init; }

}
