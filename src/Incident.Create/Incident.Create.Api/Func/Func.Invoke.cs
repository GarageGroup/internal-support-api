using GGroupp.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Internal.Support;

partial class IncidentCreateFunc
{
    public ValueTask<Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>> InvokeAsync(
        IncidentCreateIn input, CancellationToken cancellationToken = default)
        =>
        AsyncPipeline.Pipe(
            input, cancellationToken)
        .HandleCancellation()
        .Pipe(
            static @in => new DataverseEntityCreateIn<IncidentJsonCreateIn>(
                entityPluralName: "incidents",
                selectFields: selectedFields,
                entityData: new(
                    ownerId: $"/systemusers({@in.OwnerId:D})",
                    customerId: $"/accounts({@in.CustomerId:D})",
                    contactId: @in.ContactId is null ? null : $"/contacts({@in.ContactId})",
                    title: @in.Title,
                    description: @in.Description,
                    caseTypeCode: @in.CaseTypeCode switch
                    {
                        IncidentCaseTypeCode.Question   => 1,
                        IncidentCaseTypeCode.Problem    => 2,
                        IncidentCaseTypeCode.Request    => 3,
                        _ => null
                    },
                    priorityCode: @in.PriorityCode switch
                    {
                        IncidentPriorityCode.Hight  => 1,
                        IncidentPriorityCode.Normal => 2,
                        IncidentPriorityCode.Low    => 3,
                        _ => null
                    })))
        .PipeValue(
            GetEntityCreateSupplier(input.CallerUserId).CreateEntityAsync<IncidentJsonCreateIn, IncidentJsonCreateOut>)
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
            DataverseFailureCode.RecordNotFound     => IncidentCreateFailureCode.NotFound,
            DataverseFailureCode.UserNotEnabled     => IncidentCreateFailureCode.NotAllowed,
            DataverseFailureCode.PrivilegeDenied    => IncidentCreateFailureCode.NotAllowed,
            DataverseFailureCode.Throttling         => IncidentCreateFailureCode.TooManyRequests,
            _ => default
        };
}