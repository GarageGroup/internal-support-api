using System;

namespace GGroupp.Internal.Support;

public readonly record struct UserGetIn
{
    public UserGetIn(Guid activeDirectoryUserId)
        =>
        ActiveDirectoryUserId = activeDirectoryUserId;

    public Guid ActiveDirectoryUserId { get; }
}