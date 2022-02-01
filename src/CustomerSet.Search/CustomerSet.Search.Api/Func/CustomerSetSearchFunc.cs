using GGroupp.Infra;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Internal.Support;

internal sealed partial class CustomerSetSearchFunc : IAsyncValueFunc<CustomerSetSearchIn, Result<CustomerSetSearchOut, Failure<CustomerSetSearchFailureCode>>>
{
    private static readonly ReadOnlyCollection<string> entities;

    static CustomerSetSearchFunc()
        =>
        entities = new(new[] { "account" });

    private readonly IDataverseSearchSupplier dataverseSearchSupplier;

    private CustomerSetSearchFunc(IDataverseSearchSupplier dataverseSearchSupplier)
        =>
        this.dataverseSearchSupplier = dataverseSearchSupplier;

    public static CustomerSetSearchFunc Create(IDataverseSearchSupplier dataverseSearchSupplier)
        =>
        new(
            dataverseSearchSupplier ?? throw new ArgumentNullException(nameof(dataverseSearchSupplier)));

    public partial ValueTask<Result<CustomerSetSearchOut, Failure<CustomerSetSearchFailureCode>>> InvokeAsync(
        CustomerSetSearchIn input, CancellationToken cancellationToken = default);
}