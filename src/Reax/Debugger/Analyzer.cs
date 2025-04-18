using System;
using System.Diagnostics;

namespace Reax.Debugger;

public class Analyzer
{
    private readonly Stopwatch _sw;
    private int _g0;
    private int _g1;
    private int _g2;

    public Analyzer()
    {
        _sw = new Stopwatch();
        _g0 = 0;
        _g1 = 0;
        _g2 = 0;
    }

    public void Start() 
    {
        _sw.Start();
        _g0 = GC.CollectionCount(0);
        _g1 = GC.CollectionCount(1);
        _g2 = GC.CollectionCount(2);
    }

    public string Stop() 
    {
        _sw.Stop();
        _g0 = GC.CollectionCount(0) - _g0;
        _g1 = GC.CollectionCount(1) - _g1;
        _g2 = GC.CollectionCount(2) - _g2;

        var result = $"{_sw} | GEN 0: {_g0} | GEN 1: {_g1} | GEN 2: {_g2}";
        _sw.Reset();
        return result;
    }
}
