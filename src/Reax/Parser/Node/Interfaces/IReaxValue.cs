using Reax.Runtime;

namespace Reax.Parser.Node.Interfaces;

public interface IReaxValue : IReaxType
{
    object Value { get; }
}
