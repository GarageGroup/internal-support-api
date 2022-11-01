using GGroupp.Infra;
using System;
using System.Collections.Generic;

namespace GGroupp.Internal.Support;

using IContactSetSearchFunc = IAsyncValueFunc<ContactSetSearchIn, Result<ContactSetSearchOut, Failure<ContactSetSearchFailureCode>>>;

internal sealed partial class ContactSetSearchFunc : IContactSetSearchFunc
{
    private static readonly FlatArray<string> entities;

    static ContactSetSearchFunc()
        =>
        entities = new("contact");

    private readonly IDataverseSearchSupplier dataverseSearchSupplier;

    private ContactSetSearchFunc(IDataverseSearchSupplier dataverseSearchSupplier)
        =>
        this.dataverseSearchSupplier = dataverseSearchSupplier;

    public static ContactSetSearchFunc Create(IDataverseSearchSupplier dataverseSearchSupplier)
        =>
        new(
            dataverseSearchSupplier ?? throw new ArgumentNullException(nameof(dataverseSearchSupplier)));
}