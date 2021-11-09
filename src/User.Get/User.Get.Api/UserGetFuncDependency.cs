using GGroupp.Infra;
using PrimeFuncPack;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("GGroupp.Internal.Support.CustomerSet.Find.Api.Tests")]

namespace GGroupp.Internal.Support;

using IUserGetFunc = IAsyncValueFunc<UserGetIn, Result<UserGetOut, Failure<UserGetFailureCode>>>;

public static class UserGetFuncDependency
{
    public static Dependency<IUserGetFunc> UseUserGetApi<TDataverseApiClient>(this Dependency<TDataverseApiClient> dependency)
        where TDataverseApiClient : IDataverseEntityGetSupplier
        =>
        dependency.Map<IUserGetFunc>(apiClient => UserGetFunc.Create(apiClient));
}