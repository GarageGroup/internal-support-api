using System;

namespace GGroupp.Internal.Support
{
    public sealed record UserGetIn
    {
        public UserGetIn(Guid activeDirectoryUserId)
            =>
            ActiveDirectoryUserId = activeDirectoryUserId;

        public Guid ActiveDirectoryUserId { get; }
    }
}