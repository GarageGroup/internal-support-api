using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace GGroupp.Internal.Support;

internal sealed record class IncidentJsonCreateIn
{
    internal IncidentJsonCreateIn(
        string ownerId,
        string customerId,
        string title,
        [AllowNull] string description,
        int caseTypeCode,
        int? caseOriginCode,
        [AllowNull] string contactId)
    {
        OwnerId = ownerId.OrEmpty();
        CustomerId = customerId.OrEmpty();
        ContactId = contactId.OrNullIfEmpty();
        Title = title.OrEmpty();
        Description = description.OrNullIfEmpty();
        CaseTypeCode = caseTypeCode;
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
    public int CaseTypeCode { get; }

    [JsonPropertyName(ApiNames.CaseOriginCode)]
    public int? CaseOriginCode { get; }
}
