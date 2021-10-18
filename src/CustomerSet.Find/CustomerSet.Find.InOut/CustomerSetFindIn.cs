using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Internal.Support;

public sealed record CustomerSetFindIn
{
    public CustomerSetFindIn(
        [AllowNull] string searchText,
        int? top = null)
    {
        SearchText = searchText ?? string.Empty;
        Top = top;
    }
    public string SearchText { get; }

    public int? Top {  get; }
}