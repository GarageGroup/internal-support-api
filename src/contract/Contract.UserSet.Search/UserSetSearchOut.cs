using System;

namespace GarageGroup.Internal.Support;

public readonly record struct UserSetSearchOut
{
    public required FlatArray<UserItemSearchOut> Users { get; init; }
}