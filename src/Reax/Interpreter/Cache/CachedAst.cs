using System;
using Reax.Core.Ast;

namespace Reax.Interpreter.Cache;

public class CachedAst
{
    public DateTime LastModified { get; }
    public ReaxNode[] Data { get; }

    public CachedAst(ReaxNode[] data)
    {
        LastModified = DateTime.Now;
        Data = data;
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(LastModified.ToBinary());
        writer.Write(Data.Length);
        foreach (var node in Data)
        {
            node.Serialize(writer);
        }
    }
}
