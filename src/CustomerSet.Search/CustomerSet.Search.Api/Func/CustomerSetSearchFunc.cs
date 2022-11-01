using GGroupp.Infra;
using System;
using System.Collections.Generic;

namespace GGroupp.Internal.Support;

using ICustomerSetSearchFunc = IAsyncValueFunc<CustomerSetSearchIn, Result<CustomerSetSearchOut, Failure<CustomerSetSearchFailureCode>>>;

internal sealed partial class CustomerSetSearchFunc : ICustomerSetSearchFunc
{
    private static readonly FlatArray<string> entities;

    static CustomerSetSearchFunc()
        =>
        entities = new("account");

    private readonly IDataverseSearchSupplier dataverseSearchSupplier;

    private CustomerSetSearchFunc(IDataverseSearchSupplier dataverseSearchSupplier)
        =>
        this.dataverseSearchSupplier = dataverseSearchSupplier;

    public static CustomerSetSearchFunc Create(IDataverseSearchSupplier dataverseSearchSupplier)
        =>
        new(
            dataverseSearchSupplier ?? throw new ArgumentNullException(nameof(dataverseSearchSupplier)));
}