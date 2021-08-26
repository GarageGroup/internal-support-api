using System.Text.Json.Serialization;

namespace GGroupp.Internal.Support;

public record CreateIncidentJsonOut
{
    public CreateIncidentJsonOut(
        Guid incidentId,
        string title)
    {
        IncidentId = incidentId;
        Title = title ?? string.Empty;
    }

    [JsonPropertyName(ApiJsonFieldName.ownerIdOdataBind)]
    public Guid IncidentId{ get; init; }

    [JsonPropertyName(ApiJsonFieldName.incidentId)]
    public string Title { get; init;}
}