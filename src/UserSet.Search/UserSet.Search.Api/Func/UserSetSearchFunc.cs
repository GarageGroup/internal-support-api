using System;
using System.Collections.Generic;
using GGroupp.Infra;

namespace GGroupp.Internal.Support;

using IUserSetSearchFunc = IAsyncValueFunc<UserSetSearchIn, Result<UserSetSearchOut, Failure<UserSetSearchFailureCode>>>;

internal sealed partial class UserSetSearchFunc : IUserSetSearchFunc
{
    public static UserSetSearchFunc Create(IDataverseSearchSupplier dataverseSearchSupplier)
        =>
        new(
            dataverseSearchSupplier ?? throw new ArgumentNullException(nameof(dataverseSearchSupplier)));

    private static readonly FlatArray<string> entities;

    static UserSetSearchFunc()
        =>
        entities = new("systemuser");

    private readonly IDataverseSearchSupplier dataverseSearchSupplier;

    private UserSetSearchFunc(IDataverseSearchSupplier dataverseSearchSupplier)
        =>
        this.dataverseSearchSupplier = dataverseSearchSupplier;
}