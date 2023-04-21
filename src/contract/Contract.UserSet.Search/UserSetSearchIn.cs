using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Internal.Support;

public sealed record class UserSetSearchIn
{
    public UserSetSearchIn([AllowNull] string searchText)
        =>
        SearchText = searchText ?? string.Empty;

    public string SearchText { get; }

    public int? Top {  get; init; }
}