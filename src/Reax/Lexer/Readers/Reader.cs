using System;

namespace Reax.Lexer.Readers;

public interface IReader
{
    public bool EndOfFile { get; }
    public char BeforeChar { get; }
    public char CurrentChar { get; }
    public char NextChar { get; }
    public int Position { get; }
    public void Advance();
    public string GetString(int start, int end);
}
