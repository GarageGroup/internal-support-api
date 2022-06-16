using System;
using GGroupp.Infra;
using PrimeFuncPack;

namespace GGroupp.Internal.Support;

using IUserSetSearchFunc = IAsyncValueFunc<UserSetSearchIn, Result<UserSetSearchOut, Failure<UserSetSearchFailureCode>>>;

public static class UserSetSearchFuncDependency
{
    public static Dependency<IUserSetSearchFunc> UseUserSetSearchApi<TDataverseApiClient>(this Dependency<TDataverseApiClient> dependency)
        where TDataverseApiClient : IDataverseSearchSupplier
    {
        _ = dependency ?? throw new ArgumentNullException(nameof(dependency));

        return dependency.Map<IUserSetSearchFunc>(CreateFunc);

        static UserSetSearchFunc CreateFunc(TDataverseApiClient apiClient)
            =>
            UserSetSearchFunc.Create(apiClient);
    }
}