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

    [JsonPropertyName(ApiJsonFieldName.IncidentId)]
    public Guid IncidentId{ get; init; }

    [JsonPropertyName(ApiJsonFieldName.Title)]
    public string Title { get; init;}
}