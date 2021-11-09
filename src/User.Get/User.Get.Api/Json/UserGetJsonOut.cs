using System;
using System.Text.Json.Serialization;

namespace GGroupp.Internal.Support;

internal sealed record class UserGetJsonOut
{
    [JsonPropertyName(ApiNames.SystemUserId)]
    public Guid SystemUserId { get; init; }
}
