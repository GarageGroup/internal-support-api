using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Internal.Support;

public readonly record struct CustomerSetSearchIn
{
    private readonly string? searchText;

    public CustomerSetSearchIn([AllowNull] string searchText, int? top = null)
    {
        this.searchText = string.IsNullOrEmpty(searchText) ? null : searchText;
        Top = top;
    }

    public string SearchText => searchText ?? string.Empty;

    public int? Top { get; }
}