using System.Text.Json.Serialization;

namespace GGroupp.Internal.Support;

public record UserGetJsonOut
{
    public UserGetJsonOut(Guid systemUserId)
        =>
        SystemUserId = systemUserId;

    [JsonPropertyName(ApiJsonFieldName.SystemUserId)]
    public Guid SystemUserId{ get; init; }
}