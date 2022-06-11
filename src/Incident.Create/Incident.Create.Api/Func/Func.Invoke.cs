using GGroupp.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;
using static System.FormattableString;

namespace GGroupp.Internal.Support;

partial class IncidentCreateFunc
{
    public ValueTask<Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>> InvokeAsync(
        IncidentCreateIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input, cancellationToken)
        .HandleCancellation()
        .Pipe(
            static @in => new DataverseEntityCreateIn<IncidentJsonCreateIn>(
                entityPluralName: "incidents",
                selectFields: selectedFields,
                entityData: new(
                    ownerId: Invariant($"/systemusers({@in.OwnerId:D})"),
                    customerId: Invariant($"/accounts({@in.CustomerId:D})"),
                    title: @in.Title,
                    description: @in.Description,
                    caseTypeCode: @in.CaseTypeCode,
                    caseOriginCode: @in.CaseOriginCode,
                    contactId: @in.ContactId is null ? null :$"/contacts({@in.ContactId})")))
        .PipeValue(
            entityCreateSupplier.CreateEntityAsync<IncidentJsonCreateIn, IncidentJsonCreateOut>)
        .MapFailure(
            static failure => failure.MapFailureCode(MapDataverseFailureCode))
        .MapSuccess(
            static entityCreateOut => new IncidentCreateOut(
                id: entityCreateOut.Value.IncidentId,
                title: entityCreateOut.Value.Title));

    private static IncidentCreateFailureCode MapDataverseFailureCode(DataverseFailureCode dataverseFailureCode)
        =>
        dataverseFailureCode switch
        {
            DataverseFailureCode.RecordNotFound => IncidentCreateFailureCode.NotFound,
            DataverseFailureCode.PicklistValueOutOfRange => IncidentCreateFailureCode.UnexpectedCaseCode,
            DataverseFailureCode.UserNotEnabled => IncidentCreateFailureCode.NotAllowed,
            DataverseFailureCode.PrivilegeDenied => IncidentCreateFailureCode.NotAllowed,
            DataverseFailureCode.Throttling => IncidentCreateFailureCode.TooManyRequests,
            _ => default
        };
}