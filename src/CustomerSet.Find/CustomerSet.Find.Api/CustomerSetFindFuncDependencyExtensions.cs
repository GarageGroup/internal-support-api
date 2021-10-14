using GGroupp.Infra;
using PrimeFuncPack;
using System;

namespace GGroupp.Internal.Support;

using ICustomerSetFind = IAsyncValueFunc<CustomerSetFindIn, Result<CustomerSetFindOut, Failure<CustomerSetFindFailureCode>>>;

public static class CustomerSetFindFuncDependencyExtensions
{
    public static Dependency<ICustomerSetFind> UseCustomerSetFindApi<TDataverseApiClient>(this Dependency<TDataverseApiClient> dependency)
        where TDataverseApiClient : IDataverseEntitySetGetSupplier
        =>
        dependency.Map<ICustomerSetFind>(apiClient => CustomerSetFindFunc.Create(apiClient));
}