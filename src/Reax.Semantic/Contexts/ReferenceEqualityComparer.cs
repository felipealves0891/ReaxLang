using System;
using System.Diagnostics.CodeAnalysis;

namespace Reax.Semantic.Contexts;

public class ReferenceEqualityComparer : IEqualityComparer<Reference>
{
    public bool Equals(Reference? x, Reference? y)
    {
        return x is not null
            && y is not null
            && x.Equals(y);
    }

    public int GetHashCode([DisallowNull] Reference obj)
    {
        return obj.GetHashCode();
    }
}
