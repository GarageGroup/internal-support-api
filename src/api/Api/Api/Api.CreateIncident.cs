using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

partial class SupportApi
{
    public ValueTask<Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>> CreateIncidentAsync(
        IncidentCreateIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input ?? throw new ArgumentNullException(nameof(input)), cancellationToken)
        .HandleCancellation()
        .Pipe(
            static @in => new DataverseEntityCreateIn<IncidentJsonCreateIn>(
                entityPluralName: IncidentJsonApiNames.EntityPluralName,
                selectFields: IncidentCreateSelectedFields,
                entityData: new()
                {
                    OwnerId = $"/systemusers({@in.OwnerId:D})",
                    CustomerId = $"/accounts({@in.CustomerId:D})",
                    ContactId = @in.ContactId is null ? null : $"/contacts({@in.ContactId})",
                    Title = @in.Title,
                    Description = @in.Description,
                    CaseTypeCode = @in.CaseTypeCode switch
                    {
                        IncidentCaseTypeCode.Question   => 1,
                        IncidentCaseTypeCode.Problem    => 2,
                        IncidentCaseTypeCode.Request    => 3,
                        _ => null
                    },
                    PriorityCode = @in.PriorityCode switch
                    {
                        IncidentPriorityCode.Hight  => 1,
                        IncidentPriorityCode.Normal => 2,
                        IncidentPriorityCode.Low    => 3,
                        _ => null
                    }
                }))
        .PipeValue(
            GetEntityCreateSupplier(input.CallerUserId).CreateEntityAsync<IncidentJsonCreateIn, IncidentJsonCreateOut>)
        .MapFailure(
            static failure => failure.MapFailureCode(ToIncidentCreateFailureCode))
        .MapSuccess(
            static entityCreateOut => new IncidentCreateOut(
                id: entityCreateOut.Value.IncidentId,
                title: entityCreateOut.Value.Title));

    private static IncidentCreateFailureCode ToIncidentCreateFailureCode(DataverseFailureCode dataverseFailureCode)
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