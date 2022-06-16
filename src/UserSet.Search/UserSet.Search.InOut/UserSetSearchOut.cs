using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Internal.Support;

public sealed record class UserSetSearchOut
{
    public UserSetSearchOut([AllowNull] IReadOnlyCollection<UserItemSearchOut> users)
        =>
        Users = users ?? Array.Empty<UserItemSearchOut>();

    public IReadOnlyCollection<UserItemSearchOut> Users { get; }
}