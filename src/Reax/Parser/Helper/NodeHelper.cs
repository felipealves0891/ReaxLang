using System;
using Reax.Parser.Node;

namespace Reax.Parser.Helper;

public static class NodeHelper
{
    public static ReaxNode[] ArrayConcat(this ReaxNode[] source, params ReaxNode[] nodes)
    {
        return source.Concat(nodes).ToArray();
    }
}
