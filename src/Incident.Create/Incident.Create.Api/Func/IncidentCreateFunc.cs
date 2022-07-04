using GGroupp.Infra;
using System;
using System.Collections.ObjectModel;

namespace GGroupp.Internal.Support;

using IIncidentCreateFunc = IAsyncValueFunc<IncidentCreateIn, Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>>;

internal sealed partial class IncidentCreateFunc : IIncidentCreateFunc
{
    public static IncidentCreateFunc Create<TApiClient>(TApiClient apiClient)
        where TApiClient : class, IDataverseEntityCreateSupplier, IDataverseImpersonateSupplier<TApiClient>
    {
        _ = apiClient ?? throw new ArgumentNullException(nameof(apiClient));

        return new(apiClient, apiClient);
    }

    private static readonly ReadOnlyCollection<string> selectedFields;

    static IncidentCreateFunc()
        =>
        selectedFields = new(new[] { ApiNames.IncidentId, ApiNames.Title });

    private readonly IDataverseEntityCreateSupplier entityCreateSupplier;

    private readonly IDataverseImpersonateSupplier<IDataverseEntityCreateSupplier> impersonateSupplier;

    private IncidentCreateFunc(
        IDataverseEntityCreateSupplier entityCreateSupplier,
        IDataverseImpersonateSupplier<IDataverseEntityCreateSupplier> impersonateSupplier)
    {
        this.entityCreateSupplier = entityCreateSupplier;
        this.impersonateSupplier = impersonateSupplier;
    }

    private IDataverseEntityCreateSupplier GetEntityCreateSupplier(Guid? callerUserId)
        =>
        callerUserId switch
        {
            null => entityCreateSupplier,
            _ => impersonateSupplier.Impersonate(callerUserId.Value)
        };
}