using System;
using Reax.Core.Ast;
using Reax.Core.Ast.Expressions;
using Reax.Core.Ast.Literals;
using Reax.Core.Ast.Statements;
using Reax.Core.Types;

namespace Reax.Core;

public interface IReaxInterpreter
{
    void Define(string script, string identifier, ReaxNode[] nodes);

    void Declare(string identifier, DataType dataType, bool immutable, bool async);

    void Assign(VarNode identifier, LiteralNode value);
    
    void Binding(string identifier, DataType dataType, ExpressionNode expression);

    void Observer(string identifier, StatementNode expression);

    void Observer(string identifier, StatementNode expression, BinaryNode condition);

    LiteralNode Resolve(VarNode variable);

    LiteralNode Invoke(string script, string function, LiteralNode[] parameters);

    LiteralNode Invoke(string function, LiteralNode[] parameters);
}
