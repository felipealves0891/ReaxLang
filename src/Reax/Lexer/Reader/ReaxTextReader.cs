using System;

namespace Reax.Lexer.Reader;

public class ReaxTextReader : IReader
{
    private readonly string _source;
    private readonly string _filename;
    private int _position;

    public ReaxTextReader(string source, string filename)
    {
        _source = source;
        _filename = filename;
        _position = 0;
    }

    public bool EndOfFile => Position >= _source.Length;
    public byte BeforeChar => Position > 0 ? (byte)_source[Position-1] : (byte)' ';
    public byte CurrentChar => (byte)_source[Position];
    public byte NextChar => (byte)_source[Position+1];
    public int Position => _position;
    public string FileName => _filename;

    public void Advance() 
    {
        if(EndOfFile)
            throw new InvalidOperationException("Não é possivel avançar após o fim do arquivo!");

        _position++;
    }

    public byte[] GetString(int start, int end)
    {
        return _source[start..end].ToCharArray().Select(x => (byte)x).ToArray();
    }
}
