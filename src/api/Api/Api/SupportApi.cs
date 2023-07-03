using System;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

internal sealed partial class SupportApi : ISupportApi
{
    private static readonly FlatArray<string> IncidentCreateSelectedFields;

    private static readonly FlatArray<string> ContactSetSearchEntities;

    private static readonly FlatArray<string> CustomerSetSearchEntities;

    private static readonly FlatArray<string> UseSetSearchEntities;

    static SupportApi()
    {
        ContactSetSearchEntities = new("contact");
        CustomerSetSearchEntities = new("account");
        UseSetSearchEntities = new("systemuser");
        IncidentCreateSelectedFields = new(IncidentJsonApiNames.IncidentId, IncidentJsonApiNames.Title);
    }

    private readonly IDataverseApiClient dataverseApiClient;

    internal SupportApi(IDataverseApiClient dataverseApiClient)
        =>
        this.dataverseApiClient = dataverseApiClient;

    private IDataverseEntityCreateSupplier GetEntityCreateSupplier(Guid? callerUserId)
        =>
        callerUserId switch
        {
            null => dataverseApiClient,
            _ => dataverseApiClient.Impersonate(callerUserId.Value)
        };
}