using System;
using System.Text.Json.Serialization;

namespace GGroupp.Internal.Support;

internal sealed record class IncidentJsonCreateIn
{
    internal IncidentJsonCreateIn(
        string ownerId,
        string customerId,
        string title,
        string description,
        int caseTypeCode,
        int? caseOriginCode,
        string? contactId)
    {
        OwnerId = ownerId.OrEmpty();
        CustomerId = customerId.OrEmpty();
        ContactId = contactId;
        Title = title.OrEmpty();
        Description = description.OrEmpty();
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
    public string Description { get; }

    [JsonPropertyName(ApiNames.CaseTypeCode)]
    public int CaseTypeCode { get; }

    [JsonPropertyName(ApiNames.CaseOriginCode)]
    public int? CaseOriginCode { get; }
}
