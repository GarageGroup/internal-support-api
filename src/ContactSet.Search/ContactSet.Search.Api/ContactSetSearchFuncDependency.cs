using GGroupp.Infra;
using PrimeFuncPack;
using System;

namespace GGroupp.Internal.Support;

using IContactSearch = IAsyncValueFunc<ContactSetSearchIn, Result<ContactSetSearchOut, Failure<ContactSetSearchFailureCode>>>;

public static class ContactSetSearchFuncDependency
{
    public static Dependency<IContactSearch> UseContactSetSearchApi<TDataverseApiClient>(
        this Dependency<TDataverseApiClient> dependency)
        where TDataverseApiClient : IDataverseSearchSupplier
        =>
        dependency.Map<IContactSearch>(apiClient => ContactSetSearchFunc.Create(apiClient));
}