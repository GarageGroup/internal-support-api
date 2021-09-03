using GGroupp.Infra;
using PrimeFuncPack;

namespace GGroupp.Internal.Support;

using IUserGetFunc = IAsyncValueFunc<UserGetIn, Result<UserGetOut, Failure<UserGetFailureCode>>>;

public static class UserGetFuncDependencyExtensions
{
    public static Dependency<IUserGetFunc> UseUserGetApi<TDataverseApiClient>(
        this Dependency<TDataverseApiClient> dependency)
        where TDataverseApiClient : IDataverseEntityGetSupplier
        =>
        dependency.Map<IUserGetFunc>(apiClient => UserGetFunc.Create(apiClient));
}