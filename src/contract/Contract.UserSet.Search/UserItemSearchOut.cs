using System;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Internal.Support;

public sealed record class UserItemSearchOut
{
    public UserItemSearchOut(Guid id, [AllowNull] string fullName)
    {
        Id = id;
        FullName = fullName ?? string.Empty;
    }

    public Guid Id { get; }

    public string FullName { get; }
}