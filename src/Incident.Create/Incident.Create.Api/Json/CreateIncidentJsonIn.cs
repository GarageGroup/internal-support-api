using System.Text.Json.Serialization;

namespace GGroupp.Internal.Support;

public record CreateIncidentJsonIn
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

    [JsonPropertyName(ApiJsonFieldName.OwnerIdOdataBind)]
    public string OwnerId { get; init; }

    [JsonPropertyName(ApiJsonFieldName.CustomerIdOdataBind)]
    public string CustomerId { get; init;}

    [JsonPropertyName(ApiJsonFieldName.Title)]
    public string Title { get; init; }

    [JsonPropertyName(ApiJsonFieldName.Description)]
    public string Description { get; init;}
}