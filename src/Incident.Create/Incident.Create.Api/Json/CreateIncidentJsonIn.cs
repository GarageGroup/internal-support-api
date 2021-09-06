using System.Text.Json.Serialization;

namespace GGroupp.Internal.Support;

public sealed record CreateIncidentJsonIn
{
    public CreateIncidentJsonIn(
        string ownerId,
        string customerId,
        string title,
        string description)
    {
        OwnerId = ownerId;
        CustomerId = customerId;
        Title = title ?? string.Empty;
        Description = description ?? string.Empty;
    }

    [JsonPropertyName(ApiNames.OwnerIdOdataBind)]
    public string OwnerId { get; init; }

    [JsonPropertyName(ApiNames.CustomerIdOdataBind)]
    public string CustomerId { get; init; }

    [JsonPropertyName(ApiNames.Title)]
    public string Title { get; init; }

    [JsonPropertyName(ApiNames.Description)]
    public string Description { get; init; }
}
