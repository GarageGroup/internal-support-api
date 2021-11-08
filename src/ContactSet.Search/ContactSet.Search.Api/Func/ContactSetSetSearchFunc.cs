using GGroupp.Infra;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Internal.Support;

internal sealed partial class ContactSetSearchFunc : IAsyncValueFunc<ContactSetSearchIn, Result<ContactSetSearchOut, Failure<ContactSetSearchFailureCode>>>
{
    private static readonly ReadOnlyCollection<string> entities;

    static ContactSetSearchFunc()
        =>
        entities = new(new[] { "contact" });

    private readonly IDataverseSearchSupplier dataverseSearchSupplier;

    private ContactSetSearchFunc(IDataverseSearchSupplier dataverseEntityCreateSupplier)
        =>
        this.dataverseSearchSupplier = dataverseEntityCreateSupplier;

    public static ContactSetSearchFunc Create(IDataverseSearchSupplier dataverseEntitySearchSupplier)
        =>
        new(
            dataverseEntitySearchSupplier ?? throw new ArgumentNullException(nameof(dataverseEntitySearchSupplier)));

    public partial ValueTask<Result<ContactSetSearchOut, Failure<ContactSetSearchFailureCode>>> InvokeAsync(
        ContactSetSearchIn input, CancellationToken cancellationToken = default);
}