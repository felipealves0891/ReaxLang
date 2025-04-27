using System;
using Reax.Interpreter;
using Reax.Parser;
using Reax.Parser.Node;
using Reax.Runtime;
using Reax.Runtime.Functions;

namespace Reax.Tests.Runtime;

public class ReaxExecutionContextTests
{
    [Fact]
    public void DeclareVariable_ShouldRegisterVariable()
    {
        var context = new ReaxExecutionContext("root");
        context.DeclareVariable("x", isAsync: false);

        // Usa reflection ou métodos públicos para validar se foi declarada
        var ex = Record.Exception(() => context.SetVariable("x", new NullNode(new SourceLocation())));
        Assert.Null(ex);
    }

    [Fact]
    public void DeclareImmutable_ShouldPreventReassignment()
    {
        var context = new ReaxExecutionContext("root");
        context.DeclareImmutable("x", new NullNode(new SourceLocation()));

        var ex = Assert.Throws<InvalidOperationException>(() => 
            context.SetVariable("x", new NullNode(new SourceLocation()))
        );

        Assert.Contains("imutavel", ex.Message);
    }

    [Fact]
    public void SetVariable_ShouldThrowIfNotDeclared()
    {
        var context = new ReaxExecutionContext("root");

        var ex = Assert.Throws<InvalidOperationException>(() => 
            context.SetVariable("x", new NullNode(new SourceLocation()))
        );

        Assert.Contains("não declarada", ex.Message);
    }

    [Fact]
    public void GetVariable_ShouldReturnCorrectValue()
    {
        var nullNode = new NullNode(new SourceLocation());
        var context = new ReaxExecutionContext("root");
        context.DeclareVariable("x", false);
        context.SetVariable("x", nullNode);

        var result = context.GetVariable("x");

        Assert.IsType<NullNode>(result);
        Assert.Equal(nullNode, result);
    }

    [Fact]
    public void ParentContext_ShouldBeSearchedWhenVariableNotFound()
    {
        var nullNode = new NullNode(new SourceLocation());
        var parent = new ReaxExecutionContext("parent");
        parent.DeclareVariable("x", false);
        parent.SetVariable("x", nullNode);

        var child = new ReaxExecutionContext("child", parent);

        var result = child.GetVariable("x");

        Assert.IsType<NullNode>(result);
        Assert.Equal(result, nullNode);
    }

    [Fact]
    public void Declare_ShouldThrowOnDuplicate()
    {
        var context = new ReaxExecutionContext("root");
        context.Declare("x");

        var ex = Assert.Throws<InvalidOperationException>(() => context.Declare("x"));
        Assert.Contains("já foi declarado", ex.Message);
    }

    [Fact]
    public void GetParent_ShouldReturnParent_WhenExists()
    {
        var parent = new ReaxExecutionContext("parent");
        var child = new ReaxExecutionContext("child", parent);

        var result = child.GetParent();

        Assert.Equal(parent, result);
    }

    [Fact]
    public void GetParent_ShouldThrow_WhenNoParent()
    {
        var context = new ReaxExecutionContext("root");

        var ex = Assert.Throws<InvalidOperationException>(() => context.GetParent());
        Assert.Contains("contexto pai", ex.Message);
    }

    [Fact]
    public void SetFunction_WithFakeFunction_ShouldBeRetrievable()
    {
        var context = new ReaxExecutionContext("root");
        context.Declare("myFunc");
        context.SetFunction("myFunc", new FakeFunction());

        var func = context.GetFunction("myFunc");
        var result = func.Invoke(new NullNode(new SourceLocation()));

        Assert.IsType<NullNode>(result);
    }

    [Fact]
    public void SetScript_ShouldStoreInterpreter()
    {
        var context = new ReaxExecutionContext("root");
        context.Declare("myScript");
        var interpreter = new ReaxInterpreter([]);

        context.SetScript("myScript", interpreter);

        var script = context.GetScript("myScript");
        Assert.Same(interpreter, script);
    }

    [Fact]
    public void SetBind_ShouldTriggerInterpretAndReturnOutput()
    {
        var context = new ReaxExecutionContext("root");
        context.Declare("myBind");
        
        var source = new SourceLocation();
        var nullNode = new StringNode("teste", source);
        var returnNode = new ReturnSuccessNode(nullNode, source);

        var interpreter = new ReaxInterpreter([returnNode]);
        context.SetBind("myBind", interpreter);

        var result = context.GetBind("myBind");

        Assert.NotNull(result);
        Assert.Equal(result, nullNode);
    }

    [Fact]
    public void SetModule_ShouldReturnFunctionFromModule()
    {
        var context = new ReaxExecutionContext("root");
        context.Declare("myModule");

        var module = new Dictionary<string, Function>
        {
            { "length", new FakeFunction() }
        };

        context.SetModule("myModule", module);

        var result = context.GetModule("myModule", "length").Invoke(new NullNode(new SourceLocation()));
        Assert.NotNull(result);
        Assert.IsType<NullNode>(result);
    }

    [Fact]
    public void GetFunction_ShouldSearchParentContext()
    {
        var parent = new ReaxExecutionContext("parent");
        parent.Declare("sharedFunc");
        parent.SetFunction("sharedFunc", new FakeFunction());

        var child = new ReaxExecutionContext("child", parent);

        var result = child.GetFunction("sharedFunc").Invoke();
        Assert.NotNull(result);
    }

    [Fact]
    public void SetObservable_ShouldThrowOnImmutable()
    {
        var context = new ReaxExecutionContext("root");
        context.DeclareImmutable("x", new NullNode(new SourceLocation()));
        var interpreter = new ReaxInterpreter([]);

        var ex = Assert.Throws<InvalidOperationException>(() => 
            context.SetObservable("x", interpreter, null)
        );

        Assert.Contains("constante", ex.Message);
    }

    [Fact]
    public void SetVariable_ShouldTriggerObservable_WhenCanRunIsTrue()
    {
        var wasRun = false;

        var context = new ReaxExecutionContext("root");
        context.DeclareVariable("x", isAsync: false);

        var functionCall = new FunctionCallNode("test", [], new SourceLocation());
        var interpreter = new ReaxInterpreter([functionCall]);
        interpreter.DeclareAndSetFunction("test", new FakeFunction(parameters => 
        { 
            wasRun = true; 
            return new StringNode("", new SourceLocation());
        }));

        context.SetObservable("x", interpreter, null);
        context.SetVariable("x", new NumberNode("1", new SourceLocation()));
        Assert.True(wasRun);
    }

    [Fact]
    public void SetVariable_ShouldNotTriggerObservable_WhenCanRunIsFalse()
    {
        var wasRun = false;

        var context = new ReaxExecutionContext("root");
        context.DeclareVariable("x", isAsync: false);

        var functionCall = new FunctionCallNode("test", [], new SourceLocation());
        var interpreter = new ReaxInterpreter([functionCall]);

        interpreter.DeclareAndSetFunction("test", new FakeFunction(parameters => 
        { 
            wasRun = true; 
            return new StringNode("", new SourceLocation());
        }));

        var condition = new BinaryNode(
            new StringNode("a", new SourceLocation()),
            new EqualityNode("==", new SourceLocation()),
            new StringNode("b", new SourceLocation()),
            new SourceLocation());

        context.SetObservable("x", interpreter, condition);
        context.SetVariable("x", new NumberNode("1", new SourceLocation()));
        Assert.False(wasRun);
    }

    [Fact]
    public async Task SetVariable_Async_ShouldTriggerAllObservablesInParallel()
    {
        var counter = 0;
        
        var context = new ReaxExecutionContext("root");
        context.DeclareVariable("x", isAsync: true);

        var functionCall = new FunctionCallNode("test", [], new SourceLocation());
        var interpreter = new ReaxInterpreter([functionCall]);

        var blocker = true;
        interpreter.DeclareAndSetFunction("test", new FakeFunction(parameters => 
        { 
            while(blocker) Task.Delay(1);
            counter++; 
            return new StringNode("", new SourceLocation());
        }));

        context.SetObservable("x", interpreter, null);
        context.SetObservable("x", interpreter, null);
        context.SetObservable("x", interpreter, null);
        var task = Task.Run(() => context.SetVariable("x", new NumberNode("1", new SourceLocation())));

        await Task.Delay(100);
        Assert.Equal(0, counter);

        blocker = false;
        await task;
        Assert.Equal(3, counter);
    }
}
