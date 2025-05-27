using System;
using Reax.Core.Ast;
using Reax.Core.Ast.Statements;
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
        LastModified = DateTime.FromBinary(reader.ReadInt64());
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
        writer.Write(LastModified.ToBinary());
        writer.Write(Data.Length);
        foreach (var node in Data)
        {
            if (node is ScriptNode scriptNode)
            {
                var fileRef = GetFileRef(scriptNode);
                fileRef.Serialize(writer);
            }
            else
            {
                node.Serialize(writer);
            }
        }

    }

    private FileRef GetFileRef(ScriptNode scriptNode)
    {
        return new FileRef(
            scriptNode.Filename,
            scriptNode.Identifier,
            scriptNode.Location);
    }
}
