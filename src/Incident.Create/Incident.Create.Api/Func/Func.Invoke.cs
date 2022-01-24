using GGroupp.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;
using static System.FormattableString;

namespace GGroupp.Internal.Support;

partial class IncidentCreateFunc
{
    public partial ValueTask<Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>> InvokeAsync(
        IncidentCreateIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input, cancellationToken)
        .Pipe(
            @in => new DataverseEntityCreateIn<IncidentJsonCreateIn>(
                entityPluralName: "incidents",
                selectFields: selectedFields,
                entityData: new(
                    ownerId: Invariant($"/systemusers({input.OwnerId:D})"),
                    customerId: Invariant($"/accounts({input.CustomerId:D})"),
                    title: input.Title,
                    description: input.Description,
                    caseTypeCode: input.CaseTypeCode,
                    caseOriginCode: input.CaseOriginCode,
                    contactId: input.ContactId is null ? null : Invariant($"/contacts({input.ContactId})"))))
        .PipeValue(
            entityCreateSupplier.CreateEntityAsync<IncidentJsonCreateIn, IncidentJsonCreateOut>)
        .MapFailure(
            failure => failure.MapFailureCode(MapDataverseFailureCode))
        .MapSuccess(
            entityCreateOut => new IncidentCreateOut(
                id: entityCreateOut.Value.IncidentId,
                title: entityCreateOut.Value.Title));

    private static IncidentCreateFailureCode MapDataverseFailureCode(int dataverseFailureCode)
        =>
        dataverseFailureCode switch
        {
            ApiNames.NotFoundFailureCode => IncidentCreateFailureCode.NotFound,
            ApiNames.PicklistValueOutOfRangeFailureCode => IncidentCreateFailureCode.UnexpectedCaseCode,
            _ => IncidentCreateFailureCode.Unknown
        };
}