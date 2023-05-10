using System.Text.Json.Serialization;

namespace GarageGroup.Internal.Support;

internal sealed record class IncidentJsonCreateIn
{
    [JsonPropertyName(IncidentJsonApiNames.OwnerIdOdataBind)]
    public string? OwnerId { get; init; }

    [JsonPropertyName(IncidentJsonApiNames.CustomerIdOdataBind)]
    public string? CustomerId { get; init; }

    [JsonPropertyName(IncidentJsonApiNames.ContactIdOdataBind)]
    public string? ContactId { get; init; }

    [JsonPropertyName(IncidentJsonApiNames.Title)]
    public string? Title { get; init; }

    [JsonPropertyName(IncidentJsonApiNames.Description)]
    public string? Description { get; init; }

    [JsonPropertyName(IncidentJsonApiNames.CaseTypeCode)]
    public int? CaseTypeCode { get; init; }

    [JsonPropertyName(IncidentJsonApiNames.PriorityCode)]
    public int? PriorityCode { get; init; }

    [JsonPropertyName(IncidentJsonApiNames.CaseOriginCode)]
    public int? CaseOriginCode { get; init; }
}