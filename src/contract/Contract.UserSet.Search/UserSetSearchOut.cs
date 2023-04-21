using System;

namespace GGroupp.Internal.Support;

public readonly record struct UserSetSearchOut
{
    public required FlatArray<UserItemSearchOut> Users { get; init; }
}