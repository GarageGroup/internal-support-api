using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Internal.Support;

public sealed record CustomerSetSearchOut
{
    public CustomerSetSearchOut([AllowNull] IReadOnlyCollection<CustomerItemSearchOut> customers)
        =>
        Customers = customers ?? Array.Empty<CustomerItemSearchOut>();

    public IReadOnlyCollection<CustomerItemSearchOut> Customers { get; }
}