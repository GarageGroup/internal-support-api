using GGroupp.Infra;
using PrimeFuncPack;
using System;

namespace GGroupp.Internal.Support;

using ICustomerSetSearchSearchFunc = IAsyncValueFunc<CustomerSetSearchIn, Result<CustomerSetSearchOut, Failure<CustomerSetSearchFailureCode>>>;

public static class CustomerSetSearchFuncDependency
{
    public static Dependency<ICustomerSetSearchSearchFunc> UseCustomerSetSearchApi<TApiClient>(this Dependency<TApiClient> dependency)
        where TApiClient : IDataverseSearchSupplier
    {
        _ = dependency ?? throw new ArgumentNullException(nameof(dependency));

        return dependency.Map<ICustomerSetSearchSearchFunc>(CreateFunc);

        static CustomerSetSearchFunc CreateFunc(TApiClient apiClient)
            =>
             CustomerSetSearchFunc.Create(apiClient);
    }
}