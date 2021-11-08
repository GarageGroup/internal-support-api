using System;
using System.Text.Json.Serialization;

namespace GGroupp.Internal.Support;

internal sealed record class UserGetJsonOut
{
    public UserGetJsonOut(Guid systemUserId)
        =>
        SystemUserId = systemUserId;

    [JsonPropertyName(ApiNames.SystemUserId)]
    public Guid SystemUserId { get; init; }
}
