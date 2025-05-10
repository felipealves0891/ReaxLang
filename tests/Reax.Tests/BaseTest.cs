using System;

namespace Reax.Tests;

public abstract class BaseTest<Tested>
{
    protected abstract Tested CreateTested();
}
