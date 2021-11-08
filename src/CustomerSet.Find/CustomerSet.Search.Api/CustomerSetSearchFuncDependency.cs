using GGroupp.Infra;
using PrimeFuncPack;
using System;

namespace GGroupp.Internal.Support;

using ICustomerSearchSearch = IAsyncValueFunc<CustomerSetFindIn, Result<CustomerSetFindOut, Failure<CustomerSetFindFailureCode>>>;

public static class CustomerSetSearchFuncDependency
{
    public static Dependency<ICustomerSearchSearch> UseCustomerSetSearchApi<TDataverseApiClient>(
        this Dependency<TDataverseApiClient> dependency)
        where TDataverseApiClient : IDataverseSearchSupplier
        =>
        dependency.Map<ICustomerSearchSearch>(apiClient => CustomerSetSearchFunc.Create(apiClient));
}