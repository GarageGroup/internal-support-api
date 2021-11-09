using System;
using System.Text.Json.Serialization;

namespace GGroupp.Internal.Support;

internal sealed record class CreateIncidentJsonOut
{
    [JsonPropertyName(ApiNames.IncidentId)]
    public Guid IncidentId{ get; init; }

    [JsonPropertyName(ApiNames.Title)]
    public string? Title { get; init; }
}
