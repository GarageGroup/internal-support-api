using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Internal.Support;

public sealed record CustomerSetFindOut
{
    public CustomerSetFindOut([AllowNull] IReadOnlyCollection<CustomerItemFindOut> customers)
        =>
        Customers = customers ?? Array.Empty<CustomerItemFindOut>();

    public IReadOnlyCollection<CustomerItemFindOut> Customers { get; }
}