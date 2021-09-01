using System.Text.Json.Serialization;

namespace GGroupp.Internal.Support;

public record CustomerSetFindJsonOut
{
    public CustomerSetFindJsonOut(
        Guid id,
        string title)
    {
        Id = id;
        Title = title ?? string.Empty;
    }

    [JsonPropertyName(ApiJsonFieldName.AccountId)]
    public Guid  Id{ get; init; }

    [JsonPropertyName(ApiJsonFieldName.Name)]
    public string Title { get; init;}
}