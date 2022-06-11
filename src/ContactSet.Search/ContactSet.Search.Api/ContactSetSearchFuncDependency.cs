using GGroupp.Infra;
using PrimeFuncPack;
using System;

namespace GGroupp.Internal.Support;

using IContactSearchFunc = IAsyncValueFunc<ContactSetSearchIn, Result<ContactSetSearchOut, Failure<ContactSetSearchFailureCode>>>;

public static class ContactSetSearchFuncDependency
{
    public static Dependency<IContactSearchFunc> UseContactSetSearchApi<TDataverseApiClient>(this Dependency<TDataverseApiClient> dependency)
        where TDataverseApiClient : IDataverseSearchSupplier
    {
        _ = dependency ?? throw new ArgumentNullException(nameof(dependency));

        return dependency.Map<IContactSearchFunc>(CreateFunc);

        static ContactSetSearchFunc CreateFunc(TDataverseApiClient apiClient)
            =>
            ContactSetSearchFunc.Create(apiClient);
    }
}