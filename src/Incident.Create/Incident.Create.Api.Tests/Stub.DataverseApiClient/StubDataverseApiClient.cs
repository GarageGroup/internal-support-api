using System;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra;

namespace GGroupp.Internal.Support.Incident.Create.Api.Tests;

internal sealed class StubDataverseApiClient : IDataverseEntityCreateSupplier, IDataverseImpersonateSupplier<StubDataverseApiClient>
{
    internal static StubDataverseApiClient Create(IDataverseEntityCreateSupplier entityCreateSupplier)
        =>
        new(entityCreateSupplier, null);

    internal static StubDataverseApiClient Create(IDataverseEntityCreateSupplier entityCreateSupplier, IFunc<Guid, Unit> impersonateAction)
        =>
        new(entityCreateSupplier, impersonateAction);

    private readonly IDataverseEntityCreateSupplier entityCreateSupplier;

    private readonly IFunc<Guid, Unit>? impersonateAction;

    private StubDataverseApiClient(IDataverseEntityCreateSupplier entityCreateSupplier, IFunc<Guid, Unit>? impersonateAction)
    {
        this.entityCreateSupplier = entityCreateSupplier;
        this.impersonateAction = impersonateAction;
    }

    public ValueTask<Result<DataverseEntityCreateOut<TOutJson>, Failure<DataverseFailureCode>>> CreateEntityAsync<TInJson, TOutJson>(
        DataverseEntityCreateIn<TInJson> input, CancellationToken cancellationToken = default)
        where TInJson : notnull
        =>
        entityCreateSupplier.CreateEntityAsync<TInJson, TOutJson>(input, cancellationToken);

    public StubDataverseApiClient Impersonate(Guid callerId)
    {
        _ = impersonateAction?.Invoke(callerId);
        return this;
    }
}