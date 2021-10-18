using System.Text.Json.Serialization;

namespace GGroupp.Internal.Support;

public sealed record CreateIncidentJsonIn
{
    public CreateIncidentJsonIn(
        string ownerId,
        string customerId,
        string title,
        string description,
        int caseTypeCode,
        int? caseOriginCode)
    {
        OwnerId = ownerId ?? string.Empty;
        CustomerId = customerId ?? string.Empty;
        Title = title ?? string.Empty;
        Description = description ?? string.Empty;
        CaseTypeCode = caseTypeCode;
        CaseOriginCode = caseOriginCode;
    }

    [JsonPropertyName(ApiNames.OwnerIdOdataBind)]
    public string OwnerId { get; init; }

    [JsonPropertyName(ApiNames.CustomerIdOdataBind)]
    public string CustomerId { get; init; }

    [JsonPropertyName(ApiNames.Title)]
    public string Title { get; init; }

    [JsonPropertyName(ApiNames.Description)]
    public string Description { get; init; }

    [JsonPropertyName(ApiNames.CaseTypeCode)]
    public int CaseTypeCode {  get; init; }

    [JsonPropertyName(ApiNames.CaseOriginCode)]
    public int? CaseOriginCode { get; init; }
}
