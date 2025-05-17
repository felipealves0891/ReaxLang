using System;
using Reax.Core.Ast.Interfaces;
using Reax.Core.Locations;

namespace Reax.Core.Ast.Objects;

public abstract record ObjectNode(SourceLocation Location) : ReaxNode(Location)
{
}
