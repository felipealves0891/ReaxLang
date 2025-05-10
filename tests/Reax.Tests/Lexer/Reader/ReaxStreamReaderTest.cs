using System;
using System.IO;
using Reax.Lexer.Reader;
using Xunit;

namespace Reax.Tests.Lexer.Reader;

public class ReaxStreamReaderTest : IDisposable
{
    private readonly string _testFilePath;
    private readonly ReaxStreamReader _reader;

    public ReaxStreamReaderTest()
    {
        _testFilePath = Path.GetTempFileName();
        File.WriteAllText(_testFilePath, "Hello\nWorld");
        _reader = new ReaxStreamReader(_testFilePath);
    }

    [Fact]
    public void FileName_ShouldReturnCorrectName()
    {
        Assert.Equal(_testFilePath, _reader.FileName);
    }

    [Fact]
    public void EndOfFile_ShouldReturnFalseInitially()
    {
        Assert.False(_reader.EndOfFile);
    }

    [Fact]
    public void Advance_ShouldMovePosition()
    {
        var initialPosition = _reader.Position;
        _reader.Advance();
        Assert.True(_reader.Position > initialPosition);
    }

    [Fact]
    public void GetString_ShouldReturnCorrectData()
    {
        var result = _reader.GetString(0, 5);
        Assert.Equal("Hello", System.Text.Encoding.UTF8.GetString(result));
    }

    [Fact]
    public void CurrentChar_ShouldReturnCorrectCharacter()
    {
        var charValue = (char)_reader.CurrentChar;
        Assert.Equal('H', charValue);
    }

    [Fact]
    public void NextChar_ShouldReturnNextCharacter()
    {
        var charValue = (char)_reader.NextChar;
        Assert.Equal('e', charValue);
    }

    [Fact]
    public void BeforeChar_ShouldReturnPreviousCharacter()
    {
        _reader.Advance();
        var charValue = (char)_reader.BeforeChar;
        Assert.Equal('H', charValue);
    }

    [Fact]
    public void Advance_EndOfFile_ShouldThrowException()
    {
        while (!_reader.EndOfFile)
        {
            _reader.Advance();
        }
        
        Assert.Throws<InvalidOperationException>(() => _reader.Advance());
    }

    public void Dispose()
    {
        _reader?.Dispose();
        File.Delete(_testFilePath);
    }
}