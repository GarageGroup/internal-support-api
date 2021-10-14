using System;
using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Internal.Support;

public sealed record IncidentCreateIn
{
    public IncidentCreateIn(
        Guid ownerId,
        Guid customerId,
        string title,
        [AllowNull] string description,
        int? caseOriginCode)
    {
        OwnerId = ownerId;
        CustomerId = customerId;
        Title = title ?? string.Empty;
        Description = description ?? string.Empty;
        CaseOriginCode = caseOriginCode;
    }

    public Guid OwnerId { get; }

    public Guid CustomerId { get; }

    public string Title { get; }

    public string Description { get; }

    public int? CaseOriginCode {  get; }
}