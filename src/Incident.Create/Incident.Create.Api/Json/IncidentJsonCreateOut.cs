using System;
using System.Text.Json.Serialization;

namespace GGroupp.Internal.Support;

internal readonly record struct IncidentJsonCreateOut
{
    [JsonPropertyName(ApiNames.IncidentId)]
    public Guid IncidentId { get; init; }

    [JsonPropertyName(ApiNames.Title)]
    public string? Title { get; init; }
}
