using System;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra;

namespace GGroupp.Internal.Support.Api.Test;

internal sealed class StubDataverseApiClient : IDataverseApiClient
{
    private readonly IDataverseEntityCreateSupplier entityCreateSupplier;

    private readonly IFunc<Guid, Unit>? impersonateAction;

    internal StubDataverseApiClient(IDataverseEntityCreateSupplier entityCreateSupplier, IFunc<Guid, Unit>? impersonateAction = null)
    {
        this.entityCreateSupplier = entityCreateSupplier;
        this.impersonateAction = impersonateAction;
    }

    public ValueTask<Result<DataverseEntityCreateOut<TOutJson>, Failure<DataverseFailureCode>>> CreateEntityAsync<TInJson, TOutJson>(
        DataverseEntityCreateIn<TInJson> input, CancellationToken cancellationToken = default)
        where TInJson : notnull
        =>
        entityCreateSupplier.CreateEntityAsync<TInJson, TOutJson>(input, cancellationToken);

    public ValueTask<Result<Unit, Failure<DataverseFailureCode>>> CreateEntityAsync<TInJson>(
        DataverseEntityCreateIn<TInJson> input, CancellationToken cancellationToken = default)
        where TInJson : notnull
        =>
        entityCreateSupplier.CreateEntityAsync(input, cancellationToken);

    public IDataverseApiClient Impersonate(Guid callerId)
    {
        _ = impersonateAction?.Invoke(callerId);
        return this;
    }

    ValueTask<Result<DataverseEmailCreateOut, Failure<DataverseFailureCode>>> IDataverseEmailSendSupplier.CreateEmailAsync(
        DataverseEmailCreateIn input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    ValueTask<Result<Unit, Failure<DataverseFailureCode>>> IDataverseEntityDeleteSupplier.DeleteEntityAsync(
        DataverseEntityDeleteIn input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    ValueTask<Result<DataverseFetchXmlOut<TEntityJson>, Failure<DataverseFailureCode>>> IDataverseFetchXmlSupplier.FetchXmlAsync<TEntityJson>(
        DataverseFetchXmlIn input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    ValueTask<Result<DataverseEntityGetOut<TJson>, Failure<DataverseFailureCode>>> IDataverseEntityGetSupplier.GetEntityAsync<TJson>(
        DataverseEntityGetIn input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    ValueTask<Result<DataverseEntitySetGetOut<TJson>, Failure<DataverseFailureCode>>> IDataverseEntitySetGetSupplier.GetEntitySetAsync<TJson>(
        DataverseEntitySetGetIn input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    ValueTask<Result<DataverseSearchOut, Failure<DataverseFailureCode>>> IDataverseSearchSupplier.SearchAsync(
        DataverseSearchIn input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    ValueTask<Result<DataverseEmailSendOut, Failure<DataverseFailureCode>>> IDataverseEmailSendSupplier.SendEmailAsync(
        DataverseEmailSendIn input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    ValueTask<Result<DataverseEntityUpdateOut<TOut>, Failure<DataverseFailureCode>>> IDataverseEntityUpdateSupplier.UpdateEntityAsync<TIn, TOut>(
        DataverseEntityUpdateIn<TIn> input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    ValueTask<Result<Unit, Failure<DataverseFailureCode>>> IDataverseEntityUpdateSupplier.UpdateEntityAsync<TInJson>(
        DataverseEntityUpdateIn<TInJson> input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    ValueTask<Result<DataverseWhoAmIOut, Failure<DataverseFailureCode>>> IDataverseWhoAmISupplier.WhoAmIAsync(
        Unit input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}