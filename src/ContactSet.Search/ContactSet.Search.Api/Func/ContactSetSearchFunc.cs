using GGroupp.Infra;
using System;
using System.Collections.ObjectModel;

namespace GGroupp.Internal.Support;

using IContactSetSearchFunc = IAsyncValueFunc<ContactSetSearchIn, Result<ContactSetSearchOut, Failure<ContactSetSearchFailureCode>>>;

internal sealed partial class ContactSetSearchFunc : IContactSetSearchFunc
{
    private static readonly ReadOnlyCollection<string> entities;

    static ContactSetSearchFunc()
        =>
        entities = new(new[] { "contact" });

    private readonly IDataverseSearchSupplier dataverseSearchSupplier;

    private ContactSetSearchFunc(IDataverseSearchSupplier dataverseSearchSupplier)
        =>
        this.dataverseSearchSupplier = dataverseSearchSupplier;

    public static ContactSetSearchFunc Create(IDataverseSearchSupplier dataverseSearchSupplier)
        =>
        new(
            dataverseSearchSupplier ?? throw new ArgumentNullException(nameof(dataverseSearchSupplier)));
}