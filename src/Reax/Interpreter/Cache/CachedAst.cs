using System;
using Reax.Core.Ast;
using Reax.Core.Debugger;
using Reax.Core.Helpers;

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

    public CachedAst(BinaryReader reader)
    {
        var length = reader.ReadInt32();
        Data = new ReaxNode[length];
        for (var i = 0; i < length; i++)
        {
            Data[i] = BinaryDeserializerHelper.Deserialize<ReaxNode>(reader);
            Logger.LogAnalize(Data[i].ToString());
        }
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(Data.Length);
        foreach (var node in Data)
            node.Serialize(writer);
    }
}
