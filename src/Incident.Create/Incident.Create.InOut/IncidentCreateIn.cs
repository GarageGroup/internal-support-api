#nullable enable

using System;
#nullable enable

using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Internal.Support
{
    public sealed record IncidentCreateIn
    {
        public Guid ActiveDirectoryUserId { get; }

        public Guid CustomerId { get; }

        public string Title { get; }

        public string Description { get; }

        public IncidentCreateIn(
            Guid activeDirectoryUserId,
            Guid customerId,
            string title,
            [AllowNull] string description)
        {
            ActiveDirectoryUserId = activeDirectoryUserId;
            CustomerId = customerId;
            Title = title ?? string.Empty;
            Description = description ?? string.Empty;
        }

    }
}
