using GGroupp.Infra;
using PrimeFuncPack;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("GGroupp.Internal.Support.Incident.Create.Api.Tests")]

namespace GGroupp.Internal.Support;

using IIncidentCreateFunc = IAsyncValueFunc<IncidentCreateIn, Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>>;

public static class IncidentCreateFuncDependency
{
    public static Dependency<IIncidentCreateFunc> UseIncidentCreateApi<TApiClient>(this Dependency<TApiClient> dependency)
        where TApiClient : IDataverseEntityCreateSupplier
    {
        _ = dependency ?? throw new ArgumentNullException(nameof(dependency));

        return dependency.Map<IIncidentCreateFunc>(CreateFunc);

        static IncidentCreateFunc CreateFunc(TApiClient apiClient)
            =>
            IncidentCreateFunc.Create(apiClient);
    }
}