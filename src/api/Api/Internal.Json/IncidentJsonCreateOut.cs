using System;
using System.Text.Json.Serialization;

namespace GarageGroup.Internal.Support;

internal readonly record struct IncidentJsonCreateOut
{
    [JsonPropertyName(IncidentJsonApiNames.IncidentId)]
    public Guid IncidentId { get; init; }

    [JsonPropertyName(IncidentJsonApiNames.Title)]
    public string? Title { get; init; }
}