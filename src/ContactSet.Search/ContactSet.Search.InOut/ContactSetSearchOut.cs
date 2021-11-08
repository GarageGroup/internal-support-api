using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Internal.Support;

public sealed record class ContactSetSearchOut
{
    public ContactSetSearchOut([AllowNull] IReadOnlyCollection<ContactItemSearchOut> contacts)
        =>
        Contacts = contacts ?? Array.Empty<ContactItemSearchOut>();

    public IReadOnlyCollection<ContactItemSearchOut> Contacts { get; }
}