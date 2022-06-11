using GGroupp.Infra;
using System;
using System.Collections.ObjectModel;

namespace GGroupp.Internal.Support;

using ICustomerSetSearchFunc = IAsyncValueFunc<CustomerSetSearchIn, Result<CustomerSetSearchOut, Failure<CustomerSetSearchFailureCode>>>;

internal sealed partial class CustomerSetSearchFunc : ICustomerSetSearchFunc
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
}