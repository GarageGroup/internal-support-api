using System;

namespace GGroupp.Internal.Support;

public readonly record struct UserGetOut
{
    public UserGetOut(Guid systemUserId)
        =>
        SystemUserId = systemUserId;

    public Guid SystemUserId { get; }
}