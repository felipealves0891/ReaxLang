using System;
using Reax.Parser;

namespace Reax.Semantic.Node;

public struct MultiType
{
    public MultiType()
    {
        Success = DataType.NONE;
        Error = DataType.NONE;
    }

    public MultiType(DataType success, DataType error)
    {
        Success = success;
        Error = error;
    }

    public DataType Success { get; init; }
    public DataType Error { get; init; }

    public override string ToString()
    {
        return $"{Success} | {Error}";
    }
}
