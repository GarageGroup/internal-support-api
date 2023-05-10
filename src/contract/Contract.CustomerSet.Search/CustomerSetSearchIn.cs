using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Internal.Support;

public sealed record class CustomerSetSearchIn
{
    public CustomerSetSearchIn([AllowNull] string searchText)
        =>
        SearchText = searchText ?? string.Empty;

    public string SearchText { get; }

    public int? Top { get; init; }
}