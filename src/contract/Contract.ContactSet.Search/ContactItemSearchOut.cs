using System;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Internal.Support;

public sealed record class ContactItemSearchOut
{
    public ContactItemSearchOut(Guid id, [AllowNull] string fullName)
    {
        Id = id;
        FullName = fullName ?? string.Empty;
    }

    public Guid Id { get; }

    public string FullName { get; }
}