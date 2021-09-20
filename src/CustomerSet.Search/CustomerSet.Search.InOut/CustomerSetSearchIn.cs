using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Internal.Support;

public sealed record CustomerSetSearchIn
{
    public CustomerSetSearchIn([AllowNull] string searchText)
        =>
        SearchText = searchText ?? string.Empty;

    public string SearchText { get; }
}