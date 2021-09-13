using GGroupp.Infra;
using PrimeFuncPack;

namespace GGroupp.Internal.Support;

using IIncidentCreateFunc = IAsyncValueFunc<IncidentCreateIn, Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>>;

public static class IncidentCreateFuncDependencyExtensions
{
    public static Dependency<IIncidentCreateFunc> UseIncidentCreateApi<TDataverseApiClient>(this Dependency<TDataverseApiClient> dependency)
        where TDataverseApiClient : IDataverseEntityCreateSupplier
        =>
        dependency.Map<IIncidentCreateFunc>(apiClient => IncidentCreateFunc.Create(apiClient));
}