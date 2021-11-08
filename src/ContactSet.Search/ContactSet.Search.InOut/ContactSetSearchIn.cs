using System;
using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Internal.Support;

public readonly record struct ContactSetSearchIn
{
    private readonly string? searchText;

    public ContactSetSearchIn(
        [AllowNull] string searchText,
        Guid ñustomerId,
        int? top = null)
    {
        this.searchText = searchText != string.Empty ? searchText : null;
        CustomerId = ñustomerId;
        Top = top;
    }

    public string SearchText => searchText ?? string.Empty;

    public Guid CustomerId {  get; }

    public int? Top {  get; }
}