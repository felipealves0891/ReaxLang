using System;

namespace Reax.Lexer.Reader;

public interface IReader
{
    public bool EndOfFile { get; }
    public char BeforeChar { get; }
    public char CurrentChar { get; }
    public char NextChar { get; }
    public int Position { get; }
    public void Advance();
    public char[] GetString(int start, int end);
}
