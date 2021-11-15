using System;
using System.Text.Json.Serialization;

namespace GGroupp.Internal.Support;

internal readonly record struct UserJsonGetOut
{
    [JsonPropertyName(ApiNames.SystemUserId)]
    public Guid SystemUserId { get; init; }
}
