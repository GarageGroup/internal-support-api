using System.Text.Json.Serialization;

namespace GGroupp.Internal.Support;

public sealed record UserGetJsonOut
{
    public UserGetJsonOut(Guid systemUserId)
        =>
        SystemUserId = systemUserId;

    [JsonPropertyName(ApiConstants.SystemUserId)]
    public Guid SystemUserId{ get; init; }
}