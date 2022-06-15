using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace GGroupp.Internal.Support;

internal sealed record class IncidentJsonCreateIn
{
    internal IncidentJsonCreateIn(
        string ownerId,
        string customerId,
        [AllowNull] string contactId,
        string title,
        [AllowNull] string description,
        int? caseTypeCode,
        int? priorityCode,
        int? caseOriginCode = null)
    {
        OwnerId = ownerId.OrEmpty();
        CustomerId = customerId.OrEmpty();
        ContactId = contactId.OrNullIfEmpty();
        Title = title.OrEmpty();
        Description = description.OrNullIfEmpty();
        CaseTypeCode = caseTypeCode;
        PriorityCode = priorityCode;
        CaseOriginCode = caseOriginCode;
    }

    [JsonPropertyName(ApiNames.OwnerIdOdataBind)]
    public string OwnerId { get; }

    [JsonPropertyName(ApiNames.CustomerIdOdataBind)]
    public string CustomerId { get; }

    [JsonPropertyName(ApiNames.ContactIdOdataBind)]
    public string? ContactId { get; }

    [JsonPropertyName(ApiNames.Title)]
    public string Title { get; }

    [JsonPropertyName(ApiNames.Description)]
    public string? Description { get; }

    [JsonPropertyName(ApiNames.CaseTypeCode)]
    public int? CaseTypeCode { get; }

    [JsonPropertyName(ApiNames.PriorityCode)]
    public int? PriorityCode { get; }

    [JsonPropertyName(ApiNames.CaseOriginCode)]
    public int? CaseOriginCode { get; }
}
