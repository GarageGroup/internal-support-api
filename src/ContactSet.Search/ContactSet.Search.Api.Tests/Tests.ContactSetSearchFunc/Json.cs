using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GGroupp.Internal.Support.ContactSet.Search.Api.Tests;

internal sealed record JsonExt
{
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

