using System;

namespace Reax.Runtime.Contexts;

public interface IContext
{
    bool CreateModule(string name);

    string CurrentScopeName(string? module = null);
    IDisposable EnterScope(string name, string? module = null);
    string ExitScope(string? module = null);

    bool Declare(Symbol symbol, string? module = null);
    bool Update(Symbol symbol, string? module = null);
    Symbol GetSymbol(string identifier, string? module = null);
}
