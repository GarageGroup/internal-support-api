using System;

namespace GGroupp.Internal.Support;

public readonly record struct ContactSetSearchOut
{
    public required FlatArray<ContactItemSearchOut> Contacts { get; init; }
}