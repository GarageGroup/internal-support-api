using System;

namespace GGroupp.Internal.Support
{
    public sealed record UserGetOut
    {
        public UserGetOut(Guid systemUserId)
            =>
            SystemUserId = systemUserId;

        public Guid SystemUserId { get; }
    }
}