using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Internal.Support;

public sealed record class UserSetSearchOut
{
    public UserSetSearchOut([AllowNull] FlatArray<UserItemSearchOut> users)
        =>
        Users = users ?? FlatArray.Empty<UserItemSearchOut>();

    public FlatArray<UserItemSearchOut> Users { get; }
}