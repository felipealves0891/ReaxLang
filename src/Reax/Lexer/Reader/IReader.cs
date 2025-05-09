using System;

namespace Reax.Lexer.Reader;

public interface IReader
{
    public bool EndOfFile { get; }
    public byte BeforeChar { get; }
    public byte CurrentChar { get; }
    public byte NextChar { get; }
    public int Position { get; }
    public int Line { get; }
    public int Column { get; }
    public void Advance();
    public byte[] GetString(int start, int end);
    public string FileName { get; }
}
