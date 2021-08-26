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

    [JsonPropertyName(ApiJsonFieldName.ownerIdOdataBind)]
    public string OwnerId { get; init; }

    [JsonPropertyName(ApiJsonFieldName.customerIdOdataBind)]
    public string CustomerId { get; init;}

    [JsonPropertyName(ApiJsonFieldName.title)]
    public string Title { get; init; }

    [JsonPropertyName(ApiJsonFieldName.description)]
    public string Description { get; init;}
}