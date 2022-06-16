using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Internal.Support;

public readonly record struct UserSetSearchIn
{
    private readonly string? searchText;

    public UserSetSearchIn([AllowNull] string searchText, int? top)
    {
        this.searchText = string.IsNullOrEmpty(searchText) ? null : searchText;
        Top = top;
    }

    public string SearchText => searchText ?? string.Empty;

    public int? Top {  get; }
}