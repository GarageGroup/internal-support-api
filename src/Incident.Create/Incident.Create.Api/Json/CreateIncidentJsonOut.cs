using System.Text.Json.Serialization;

namespace GGroupp.Internal.Support;

public sealed record CreateIncidentJsonOut
{
    public CreateIncidentJsonOut(
        Guid incidentId,
        string title)
    {
        IncidentId = incidentId;
        Title = title ?? string.Empty;
    }

    [JsonPropertyName(ApiNames.IncidentId)]
    public Guid IncidentId{ get; init; }

    [JsonPropertyName(ApiNames.Title)]
    public string Title { get; init;}
}