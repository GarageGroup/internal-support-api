using System;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Internal.Support;

public sealed record class IncidentCreateIn
{
    public IncidentCreateIn(
        Guid ownerId,
        Guid customerId,
        Guid? contactId,
        string title,
        [AllowNull] string description,
        IncidentCaseTypeCode caseTypeCode,
        IncidentPriorityCode priorityCode)
    {
        OwnerId = ownerId;
        CustomerId = customerId;
        ContactId = contactId;
        Title = title ?? string.Empty;
        Description = description ?? string.Empty;
        CaseTypeCode = caseTypeCode;
        PriorityCode = priorityCode;
    }

    public Guid OwnerId { get; }

    public Guid CustomerId { get; }

    public Guid? ContactId { get; }

    public string Title { get; }

    public string Description { get; }

    public IncidentCaseTypeCode CaseTypeCode { get; }

    public IncidentPriorityCode PriorityCode { get; }

    public Guid? CallerUserId { get; init; }
}