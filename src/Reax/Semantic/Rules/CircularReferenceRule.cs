using System;
using Reax.Parser.Node;
using Reax.Parser.Node.Expressions;
using Reax.Parser.Node.Literals;
using Reax.Parser.Node.Statements;
using Reax.Semantic.Contexts;

namespace Reax.Semantic.Rules;

public class CircularReferenceRule : BaseRule
{
    private Reference? _from = null!;

    public CircularReferenceRule() : base()        
    {
        Handlers[typeof(BindNode)] = ApplyBindNode;
        Handlers[typeof(ObservableNode)] = ApplyObservableNode;
        Handlers[typeof(DeclarationNode)] = ApplyDeclarationNode;
        Handlers[typeof(AssignmentNode)] = ApplyAssignmentNode;
        Handlers[typeof(CalculateNode)] = ApplyCalculateNode;
    }

    protected override void PrepareApply()
    {
        _from = null!;
    }
    
    private ValidationResult ApplyBindNode(IReaxNode node)
    {
        var bind = (BindNode)node;
        _from = new Reference(bind.Identifier, typeof(BindNode), false, bind.Location);
        return ValidationResult.Success();
    }

    private ValidationResult ApplyObservableNode(IReaxNode node)
    {
        var on = (ObservableNode)node;
        _from = new Reference(on.Var.Identifier, typeof(ObservableNode), false, on.Location);
        return ValidationResult.Success();
    }

    private ValidationResult ApplyDeclarationNode(IReaxNode node)
    {
        var declaration = (DeclarationNode)node;
        var to = new Reference(declaration.Identifier, typeof(DeclarationNode), false, declaration.Location);
        Context.SetTo(to);
        return ValidationResult.Success();
    }
    
    private ValidationResult ApplyAssignmentNode(IReaxNode node)
    {
        var assignment = (AssignmentNode)node;
        var variable = assignment.Identifier;
        Context.SetTo(new Reference(variable.Identifier, typeof(VarNode), false, variable.Location));

        if(assignment.Assigned is VarNode var)
            Context.SetTo(new Reference(var.Identifier, typeof(VarNode), true, assignment.Location));
        else if(assignment.Assigned is LiteralNode)
            return ValidationResult.Success();    
        else
            WalkThroughTheTreeKnots(assignment.Assigned.Children);

        return ValidationResult.Success();
    }

    private ValidationResult ApplyCalculateNode(IReaxNode node)
    {
        var calculate = (CalculateNode)node;
        WalkThroughTheTreeKnots(calculate.Children);
        return ValidationResult.Success();
    }

    private void WalkThroughTheTreeKnots(IReaxNode[] nodes)
    {
        foreach (var node in nodes)
        {
            if(node is VarNode var)
                Context.SetTo(new Reference(var.Identifier, typeof(VarNode), false, var.Location));
            else
                WalkThroughTheTreeKnots(node.Children);
        }
    }

    public override IDisposable? PrepareScope(ISemanticContext context)
    {
        if(_from is null)
            return null;
        else
            return context.EnterFrom(_from);
    }


}
