using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Internal.Support;

public sealed record class CustomerSetSearchOut
{
    public CustomerSetSearchOut([AllowNull] FlatArray<CustomerItemSearchOut> customers)
        =>
        Customers = customers ?? FlatArray.Empty<CustomerItemSearchOut>();

    public FlatArray<CustomerItemSearchOut> Customers { get; }
}