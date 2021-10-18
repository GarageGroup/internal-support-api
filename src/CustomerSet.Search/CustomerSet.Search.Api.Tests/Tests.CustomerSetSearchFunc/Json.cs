using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GGroupp.Internal.Support.CustomerSet.Search.Api.Tests;

internal record JsonExt
{
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

