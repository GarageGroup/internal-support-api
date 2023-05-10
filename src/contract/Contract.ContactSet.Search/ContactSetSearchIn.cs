using System;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Internal.Support;

public sealed record class ContactSetSearchIn
{
    public ContactSetSearchIn([AllowNull] string searchText, Guid customerId)
    {
        SearchText = searchText ?? string.Empty;
        CustomerId = customerId;
    }

    public string SearchText { get; }

    public Guid CustomerId {  get; }

    public int? Top {  get; init; }
}