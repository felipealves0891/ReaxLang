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

    public byte BeforeChar 
    {
        get
        {
            _stream.Position--;
            var b = _stream.ReadByte();
            return (byte)b;
        }
    }

    public byte CurrentChar 
    {
        get
        {
            var b = _stream.ReadByte();
            _stream.Position--;
            return (byte)b;
        }
    }

    public byte NextChar 
    {
        get
        {
            _stream.Position++;
            var b = _stream.ReadByte();
            _stream.Position--;
            _stream.Position--;
            return (byte)b;
        }
    }

    public int Position => (int)_stream.Position;

    public void Advance()
    {
        if(EndOfFile)
            throw new InvalidOperationException("Não é possivel avançar após o fim do arquivo");

        _stream.Position++;
    }

    public byte[] GetString(int start, int end)
    {
        var currentChar = _stream.Position;
        _stream.Position = start;

        var chars = new byte[end - start];
        for (int i = 0; i < end - start; i++)
            chars[i] = (byte)_stream.ReadByte();
        
        _stream.Position = currentChar;
        return chars;

    }
}
