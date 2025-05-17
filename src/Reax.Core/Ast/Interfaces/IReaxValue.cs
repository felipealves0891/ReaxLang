using Reax.Core.Types;

namespace Reax.Core.Ast.Interfaces;

public interface IReaxValue
{
    object Value { get; }
    DataType Type { get; }
}
