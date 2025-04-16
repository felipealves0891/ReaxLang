using System;

namespace Reax.Lexer.Readers;

public class ReaxTextReader : IReader
{
    private readonly string _source;
    private int _position;

    public ReaxTextReader(string source)
    {
        _source = source;
        _position = 0;
    }

    public bool EndOfFile => Position >= _source.Length;
    public char BeforeChar => Position > 0 ? _source[Position-1] : ' ';
    public char CurrentChar => _source[Position];
    public char NextChar => _source[Position+1];
    public int Position => _position;

    public void Advance() 
    {
        if(EndOfFile)
            throw new InvalidOperationException("Não é possivel avançar após o fim do arquivo!");

        _position++;
    }

    public string GetString(int start, int end)
    {
        return _source[start..end];
    }
}
