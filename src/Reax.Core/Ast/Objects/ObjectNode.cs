using System;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Locations;
using Reax.Core.Types;

namespace Reax.Core.Ast.Objects;

public abstract record ObjectNode(SourceLocation Location)
    : ReaxNode(Location), IReaxValue
{
    public abstract object Value { get;  }

    public abstract DataType Type { get;  }
}
