using System;
using System.Runtime.CompilerServices;
using GarageGroup.Infra;
using PrimeFuncPack;

[assembly: InternalsVisibleTo("GarageGroup.Internal.Support.Api.Test")]

namespace GarageGroup.Internal.Support;

public static class SupportApiDependency
{
    public static Dependency<ISupportApi> UseSupportApi(this Dependency<IDataverseApiClient> dependency)
    {
        ArgumentNullException.ThrowIfNull(dependency);
        return dependency.Map<ISupportApi>(CreateFunc);

        static SupportApi CreateFunc(IDataverseApiClient apiClient)
        {
            ArgumentNullException.ThrowIfNull(apiClient);
            return new(apiClient);
        }
    }
}