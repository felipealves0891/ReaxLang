using System;
using System.Text;

namespace Reax.Lexer.Reader;

public class ReaxStreamReader : IReader
{
    private readonly Stream _stream;

    public ReaxStreamReader(string filename)
    {
        _stream = File.OpenRead(filename);
    }

    public bool EndOfFile => _stream.CanRead && _stream.Position >= _stream.Length;

    public char BeforeChar 
    {
        get
        {
            _stream.Position--;
            var b = _stream.ReadByte();
            return (char)b;
        }
    }

    public char CurrentChar 
    {
        get
        {
            var b = _stream.ReadByte();
            _stream.Position--;
            return (char)b;
        }
    }

    public char NextChar 
    {
        get
        {
            _stream.Position++;
            var b = _stream.ReadByte();
            _stream.Position--;
            _stream.Position--;
            return (char)b;
        }
    }

    public int Position => (int)_stream.Position;

    public void Advance()
    {
        if(EndOfFile)
            throw new InvalidOperationException("Não é possivel avançar após o fim do arquivo");

        _stream.Position++;
    }

    public char[] GetString(int start, int end)
    {
        var currentChar = _stream.Position;
        _stream.Position = start;

        var chars = new char[end - start];
        for (int i = 0; i < end - start; i++)
            chars[i] = (char)_stream.ReadByte();
        
        _stream.Position = currentChar;
        return chars;

    }
}
