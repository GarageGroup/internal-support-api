using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Internal.Support;

public sealed record class ContactSetSearchOut
{
    public ContactSetSearchOut([AllowNull] FlatArray<ContactItemSearchOut> contacts)
        =>
        Contacts = contacts ?? FlatArray.Empty<ContactItemSearchOut>();

    public FlatArray<ContactItemSearchOut> Contacts { get; }
}